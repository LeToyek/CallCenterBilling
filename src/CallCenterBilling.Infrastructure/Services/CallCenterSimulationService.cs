using Microsoft.AspNetCore.SignalR;
using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.ValueObjects;
using Microsoft.Extensions.Hosting;
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

public class CallCenterSimulationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<CallCenterHub> _hubContext;
    private readonly ILogger<CallCenterSimulationService> _logger;
    private readonly Random _random = new();

    public CallCenterSimulationService(
        IServiceProvider serviceProvider,
        IHubContext<CallCenterHub> hubContext,
        ILogger<CallCenterSimulationService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Call Center Simulation Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SimulateRandomActivity();
                await Task.Delay(TimeSpan.FromSeconds(_random.Next(5, 15)), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in simulation service");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        _logger.LogInformation("Call Center Simulation Service stopped");
    }

    private async Task SimulateRandomActivity()
    {
        using var scope = _serviceProvider.CreateScope();
        var agentService = scope.ServiceProvider.GetRequiredService<IAgentStatusService>();
        var callService = scope.ServiceProvider.GetRequiredService<ICallService>();

        try
        {
            var dashboard = await agentService.GetDashboardAsync();
            if (dashboard?.Agents?.Any() != true) return;

            var agents = dashboard.Agents.ToList();
            var activityType = _random.Next(1, 5);

            switch (activityType)
            {
                case 1: // Change agent status
                    await SimulateAgentStatusChange(agents, agentService);
                    break;
                case 2: // Start a call
                    await SimulateCallStart(agents, callService);
                    break;
                case 3: // End a call
                    await SimulateCallEnd(dashboard, callService);
                    break;
                case 4: // Update metrics
                    await SimulateMetricsUpdate(agentService);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in random activity simulation");
        }
    }

    private async Task SimulateAgentStatusChange(List<AgentCallStatusDto> agents, IAgentStatusService agentService)
    {
        var agent = agents[_random.Next(agents.Count)];
        var newStatus = GetRandomStatus(agent.Status);
        
        if (newStatus != agent.Status)
        {
            await agentService.UpdateAgentStatusAsync(agent.AgentId, newStatus);
            
            var updatedAgent = await agentService.GetAgentStatusAsync(agent.AgentId);
            await _hubContext.Clients.Group("Dashboard").SendAsync("AgentStatusChanged", updatedAgent);
            
            _logger.LogInformation($"Simulated: Agent {agent.Name} status changed to {newStatus}");
        }
    }

    private async Task SimulateCallStart(List<AgentCallStatusDto> agents, ICallService callService)
    {
        var availableAgents = agents.Where(a => a.Status == AgentStatus.Active && !a.IsOnCall).ToList();
        if (!availableAgents.Any()) return;

        var agent = availableAgents[_random.Next(availableAgents.Count)];
        var customerPhone = GenerateRandomPhone();
        var customerName = GenerateRandomCustomerName();

        var callId = await callService.StartCallAsync(agent.AgentId, customerPhone, customerName);
        var callStatus = await callService.GetCallStatusAsync(callId);
        
        await _hubContext.Clients.Group("Dashboard").SendAsync("CallStatusChanged", callStatus);
        
        _logger.LogInformation($"Simulated: Started call {callId} for agent {agent.Name}");

        // Schedule automatic end
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMinutes(_random.Next(1, 10)));
            try
            {
                var revenue = _random.Next(50, 500);
                await callService.EndCallAsync(callId, revenue, "Simulated call completed");
                
                var endedCallStatus = await callService.GetCallStatusAsync(callId);
                await _hubContext.Clients.Group("Dashboard").SendAsync("CallStatusChanged", endedCallStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error ending simulated call {callId}");
            }
        });
    }

    private async Task SimulateCallEnd(CallDashboardDto dashboard, ICallService callService)
    {
        if (dashboard.ActiveCalls?.Any() != true) return;

        var call = dashboard.ActiveCalls.ElementAt(_random.Next(dashboard.ActiveCalls.Count()));
        var revenue = _random.Next(25, 750);
        
        await callService.EndCallAsync(call.CallId, revenue, "Simulated early end");
        
        var callStatus = await callService.GetCallStatusAsync(call.CallId);
        await _hubContext.Clients.Group("Dashboard").SendAsync("CallStatusChanged", callStatus);
        
        _logger.LogInformation($"Simulated: Ended call {call.CallId} early");
    }

    private async Task SimulateMetricsUpdate(IAgentStatusService agentService)
    {
        var dashboard = await agentService.GetDashboardAsync();
        await _hubContext.Clients.Group("Dashboard").SendAsync("DashboardUpdated", dashboard);
        
        _logger.LogInformation("Simulated: Dashboard metrics updated");
    }

    private AgentStatus GetRandomStatus(AgentStatus currentStatus)
    {
        var statuses = Enum.GetValues<AgentStatus>().Where(s => s != currentStatus).ToArray();
        return statuses[_random.Next(statuses.Length)];
    }

    private string GenerateRandomPhone()
    {
        return $"+1{_random.Next(200, 999)}{_random.Next(1000000, 9999999)}";
    }

    private string GenerateRandomCustomerName()
    {
        string[] firstNames = { "John", "Jane", "Mike", "Sarah", "David", "Emma", "Chris", "Lisa", "Tom", "Anna", "Alex", "Maria", "James", "Linda" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Wilson", "Anderson" };
        return $"{firstNames[_random.Next(firstNames.Length)]} {lastNames[_random.Next(lastNames.Length)]}";
    }
}
