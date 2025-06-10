using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.ValueObjects;

namespace CallCenterBilling.Application.Interfaces;

public interface IAgentStatusService
{
    Task UpdateAgentStatusAsync(int agentId, AgentStatus status);
    Task<AgentCallStatusDto?> GetAgentStatusAsync(int agentId);
    Task<IEnumerable<AgentCallStatusDto>> GetAllAgentStatusesAsync();
    Task<CallDashboardDto> GetDashboardAsync();
}