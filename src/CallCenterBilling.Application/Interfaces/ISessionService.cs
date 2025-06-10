using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.ValueObjects;

namespace CallCenterBilling.Application.Interfaces;

public interface ISessionService
{
    Task<int> StartSessionAsync(int agentId, string connectionId);
    Task EndSessionAsync(string connectionId);
    Task UpdateHeartbeatAsync(string connectionId);
    Task CleanupExpiredSessionsAsync();
    Task<bool> IsAgentOnlineAsync(int agentId);
}
