using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Domain.ValueObjects;
using CallCenterBilling.Infrastructure.Data;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

public class CallCenterHub : Hub
{
    private readonly ISessionService _sessionService;
    private readonly IAgentStatusService _agentStatusService;
    private readonly ICallService _callService;
    private readonly ILogger<CallCenterHub> _logger;

    public CallCenterHub(
        ISessionService sessionService,
        IAgentStatusService agentStatusService,
        ILogger<CallCenterHub> logger)
    {
        _sessionService = sessionService;
        _agentStatusService = agentStatusService;
        _logger = logger;
    }

    public async Task LoginAgent(int agentId)
    {
        try
        {
            await _sessionService.StartSessionAsync(agentId, Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "Agents");
            await Groups.AddToGroupAsync(Context.ConnectionId, "Dashboard");

            _logger.LogInformation("Agent {AgentId} connected with connection {ConnectionId}", agentId, Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during agent login for agent {AgentId}", agentId);
            throw;
        }
    }

    public async Task Heartbeat()
    {
        try
        {
            await _sessionService.UpdateHeartbeatAsync(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during heartbeat for connection {ConnectionId}", Context.ConnectionId);
        }
    }

    public async Task UpdateAgentStatus(int agentId, AgentStatus status)
    {
        try
        {
            await _agentStatusService.UpdateAgentStatusAsync(agentId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating agent status for agent {AgentId}", agentId);
            throw;
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            await _sessionService.EndSessionAsync(Context.ConnectionId);
            _logger.LogInformation("Connection {ConnectionId} disconnected", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disconnection for connection {ConnectionId}", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation($"Connection {Context.ConnectionId} left group {groupName}");
    }

        public async Task StartSimulatedCall(int agentId, string customerPhone, string? customerName, int durationMinutes, int revenueAmount)
    {
        try
        {
            // Start the call
            var callId = await _callService.StartCallAsync(agentId, customerPhone, customerName);
            
            // Get call status and broadcast
            var callStatus = await _callService.GetCallStatusAsync(callId);
            await Clients.Group("Dashboard").SendAsync("CallStatusChanged", callStatus);
            
            // Update dashboard
            var dashboard = await _agentStatusService.GetDashboardAsync();
            await Clients.Group("Dashboard").SendAsync("DashboardUpdated", dashboard);
            
            _logger.LogInformation($"Simulated call {callId} started for agent {agentId}");
            
            // Schedule automatic call end
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(durationMinutes));
                try
                {
                    await _callService.EndCallAsync(callId, revenueAmount, "Simulated call completed");
                    
                    // Broadcast call end
                    var endedCallStatus = await _callService.GetCallStatusAsync(callId);
                    await Clients.Group("Dashboard").SendAsync("CallStatusChanged", endedCallStatus);
                    
                    // Update dashboard
                    var updatedDashboard = await _agentStatusService.GetDashboardAsync();
                    await Clients.Group("Dashboard").SendAsync("DashboardUpdated", updatedDashboard);
                    
                    _logger.LogInformation($"Simulated call {callId} ended automatically");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error ending simulated call {callId}");
                }
            });
            
            await Clients.Caller.SendAsync("CallStarted", callId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error starting simulated call for agent {agentId}");
            await Clients.Caller.SendAsync("Error", $"Failed to start call: {ex.Message}");
        }
    }

}