using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Domain.ValueObjects;

public class AgentStatusService : IAgentStatusService
{
    private readonly IAgentRepository _agentRepository;
    private readonly ICallRepository _callRepository;
    private readonly IRealTimeNotificationService _notificationService;

    public AgentStatusService(
        IAgentRepository agentRepository,
        ICallRepository callRepository,
        IRealTimeNotificationService notificationService)
    {
        _agentRepository = agentRepository;
        _callRepository = callRepository;
        _notificationService = notificationService;
    }

    public async Task UpdateAgentStatusAsync(int agentId, AgentStatus status)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null)
            throw new ArgumentException("Agent not found", nameof(agentId));

        switch (status)
        {
            case AgentStatus.Active:
                agent.Activate();
                break;
            case AgentStatus.OnBreak:
                agent.SetOnBreak();
                break;
            case AgentStatus.Offline:
                agent.Deactivate();
                break;
        }

        await _agentRepository.UpdateAsync(agent);

        // Notify real-time updates
        var agentDto = await GetAgentStatusAsync(agentId);
        if (agentDto != null)
        {
            await _notificationService.NotifyAgentStatusChangedAsync(agentDto);
        }
    }

    public async Task<AgentCallStatusDto?> GetAgentStatusAsync(int agentId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null) return null;

        return await MapToAgentStatusDto(agent);
    }

    public async Task<IEnumerable<AgentCallStatusDto>> GetAllAgentStatusesAsync()
    {
        var agents = await _agentRepository.GetAllAsync();
        var result = new List<AgentCallStatusDto>();

        foreach (var agent in agents)
        {
            var dto = await MapToAgentStatusDto(agent);
            result.Add(dto);
        }

        return result;
    }

    public async Task<CallDashboardDto> GetDashboardAsync()
    {
        var agentStatuses = await GetAllAgentStatusesAsync();
        var activeCalls = await _callRepository.GetActiveCallsAsync();
        var metrics = await _callRepository.GetCallMetricsAsync();

        var activeCallDtos = new List<CallStatusDto>();
        foreach (var call in activeCalls)
        {
            var agent = await _agentRepository.GetByIdAsync(call.AgentId);
            activeCallDtos.Add(new CallStatusDto
            {
                CallId = call.Id,
                AgentId = call.AgentId,
                AgentName = agent?.Name ?? "Unknown",
                CustomerPhoneNumber = call.CustomerPhoneNumber,
                CustomerName = call.CustomerName,
                Status = call.Status,
                StartTime = call.StartTime,
                Duration = call.CallDuration,
                Revenue = call.Revenue
            });
        }

        return new CallDashboardDto
        {
            Agents = agentStatuses,
            ActiveCalls = activeCallDtos,
            Metrics = metrics,
            OnlineAgents = agentStatuses.Count(a => a.Status != AgentStatus.Offline),
            AgentsOnCall = agentStatuses.Count(a => a.IsOnCall),
            LastUpdated = DateTime.UtcNow
        };
    }

    private async Task<AgentCallStatusDto> MapToAgentStatusDto(Agent agent)
    {
        var currentCall = await _callRepository.GetCurrentCallByAgentIdAsync(agent.Id);
        CallStatusDto? currentCallDto = null;

        if (currentCall != null)
        {
            currentCallDto = new CallStatusDto
            {
                CallId = currentCall.Id,
                AgentId = currentCall.AgentId,
                AgentName = agent.Name,
                CustomerPhoneNumber = currentCall.CustomerPhoneNumber,
                CustomerName = currentCall.CustomerName,
                Status = currentCall.Status,
                StartTime = currentCall.StartTime,
                Duration = currentCall.CallDuration,
                Revenue = currentCall.Revenue
            };
        }

        return new AgentCallStatusDto
        {
            AgentId = agent.Id,
            Name = agent.Name,
            Status = agent.Status,
            IsOnCall = currentCall?.IsActive == true,
            CurrentCall = currentCallDto,
            LastActiveAt = agent.LastActiveAt ?? agent.CreatedAt,
            CurrentCallDuration = currentCall?.CallDuration
        };
    }

    
}