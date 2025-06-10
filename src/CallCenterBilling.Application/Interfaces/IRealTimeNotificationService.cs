using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.ValueObjects;

namespace CallCenterBilling.Application.Interfaces;

public interface IRealTimeNotificationService
{
    Task NotifyAgentStatusChangedAsync(AgentCallStatusDto agentStatus);
    Task NotifyCallStatusChangedAsync(CallStatusDto callStatus);
    Task NotifyDashboardUpdatedAsync(CallDashboardDto dashboard);
    Task NotifyAgentConnectedAsync(int agentId);
    Task NotifyAgentDisconnectedAsync(int agentId);
}