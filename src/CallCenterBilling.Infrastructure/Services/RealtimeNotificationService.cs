using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Application.Interfaces;
namespace CallCenterBilling.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
public class RealTimeNotificationService : IRealTimeNotificationService
{
    private readonly IHubContext<CallCenterHub> _hubContext;

    public RealTimeNotificationService(IHubContext<CallCenterHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAgentStatusChangedAsync(AgentCallStatusDto agentStatus)
    {
        await _hubContext.Clients.Group("Dashboard").SendAsync("AgentStatusChanged", agentStatus);
    }

    public async Task NotifyCallStatusChangedAsync(CallStatusDto callStatus)
    {
        await _hubContext.Clients.Group("Dashboard").SendAsync("CallStatusChanged", callStatus);
    }

    public async Task NotifyDashboardUpdatedAsync(CallDashboardDto dashboard)
    {
        await _hubContext.Clients.Group("Dashboard").SendAsync("DashboardUpdated", dashboard);
    }

    public async Task NotifyAgentConnectedAsync(int agentId)
    {
        await _hubContext.Clients.Group("Dashboard").SendAsync("AgentConnected", agentId);
    }

    public async Task NotifyAgentDisconnectedAsync(int agentId)
    {
        await _hubContext.Clients.Group("Dashboard").SendAsync("AgentDisconnected", agentId);
    }
}
