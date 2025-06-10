// Application/Services/CallService.cs
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Domain.ValueObjects;

public class CallService : ICallService
{
    private readonly ICallRepository _callRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IRealTimeNotificationService _notificationService;

    public CallService(
        ICallRepository callRepository,
        IAgentRepository agentRepository,
        IRealTimeNotificationService notificationService)
    {
        _callRepository = callRepository;
        _agentRepository = agentRepository;
        _notificationService = notificationService;
    }

    public async Task<int> StartCallAsync(int agentId, string customerPhoneNumber, string? customerName = null)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null)
            throw new ArgumentException("Agent not found", nameof(agentId));

        // Check if agent already has an active call
        var existingCall = await _callRepository.GetCurrentCallByAgentIdAsync(agentId);
        if (existingCall != null)
            throw new InvalidOperationException("Agent already has an active call");

        var call = new Call
        {
            AgentId = agentId,
            CustomerPhoneNumber = customerPhoneNumber,
            CustomerName = customerName,
            CreatedAt = DateTime.UtcNow
        };

        call.Start();
        var callId = await _callRepository.AddAsync(call);

        // Update agent status
        agent.Activate();
        await _agentRepository.UpdateAsync(agent);

        // Notify real-time updates
        var callDto = MapToCallStatusDto(call, agent.Name);
        await _notificationService.NotifyCallStatusChangedAsync(callDto);

        var agentDto = await GetAgentStatusDto(agent);
        await _notificationService.NotifyAgentStatusChangedAsync(agentDto);

        return callId;
    }

    public async Task EndCallAsync(int callId, decimal? revenue = null, string? notes = null)
    {
        var call = await _callRepository.GetByIdAsync(callId);
        if (call == null)
            throw new ArgumentException("Call not found", nameof(callId));

        call.End(notes);
        call.Revenue = revenue;
        await _callRepository.UpdateAsync(call);

        // Update agent performance
        var agent = await _agentRepository.GetByIdAsync(call.AgentId);
        if (agent != null)
        {
            agent.UpdatePerformance(1, revenue ?? 0);
            await _agentRepository.UpdateAsync(agent);

            // Notify real-time updates
            var callDto = MapToCallStatusDto(call, agent.Name);
            await _notificationService.NotifyCallStatusChangedAsync(callDto);

            var agentDto = await GetAgentStatusDto(agent);
            await _notificationService.NotifyAgentStatusChangedAsync(agentDto);
        }
    }

    public async Task FailCallAsync(int callId, string? reason = null)
    {
        var call = await _callRepository.GetByIdAsync(callId);
        if (call == null)
            throw new ArgumentException("Call not found", nameof(callId));

        call.Fail(reason);
        await _callRepository.UpdateAsync(call);

        // Notify real-time updates
        var agent = await _agentRepository.GetByIdAsync(call.AgentId);
        if (agent != null)
        {
            var callDto = MapToCallStatusDto(call, agent.Name);
            await _notificationService.NotifyCallStatusChangedAsync(callDto);

            var agentDto = await GetAgentStatusDto(agent);
            await _notificationService.NotifyAgentStatusChangedAsync(agentDto);
        }
    }

    public async Task<CallStatusDto?> GetCallStatusAsync(int callId)
    {
        var call = await _callRepository.GetByIdAsync(callId);
        if (call == null) return null;

        var agent = await _agentRepository.GetByIdAsync(call.AgentId);
        return MapToCallStatusDto(call, agent?.Name ?? "Unknown");
    }

    public async Task<IEnumerable<CallStatusDto>> GetActiveCallsAsync()
    {
        var calls = await _callRepository.GetActiveCallsAsync();
        var result = new List<CallStatusDto>();

        foreach (var call in calls)
        {
            var agent = await _agentRepository.GetByIdAsync(call.AgentId);
            result.Add(MapToCallStatusDto(call, agent?.Name ?? "Unknown"));
        }

        return result;
    }

    public async Task<CallMetrics> GetCallMetricsAsync(DateTime? from = null, DateTime? to = null)
    {
        return await _callRepository.GetCallMetricsAsync(from, to);
    }

    private CallStatusDto MapToCallStatusDto(Call call, string agentName)
    {
        return new CallStatusDto
        {
            CallId = call.Id,
            AgentId = call.AgentId,
            AgentName = agentName,
            CustomerPhoneNumber = call.CustomerPhoneNumber,
            CustomerName = call.CustomerName,
            Status = call.Status,
            StartTime = call.StartTime,
            Duration = call.CallDuration,
            Revenue = call.Revenue
        };
    }

    private async Task<AgentCallStatusDto> GetAgentStatusDto(Agent agent)
    {
        var currentCall = await _callRepository.GetCurrentCallByAgentIdAsync(agent.Id);
        CallStatusDto? currentCallDto = null;

        if (currentCall != null)
        {
            currentCallDto = MapToCallStatusDto(currentCall, agent.Name);
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