using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Domain.Interfaces
{
    public interface IAgentSessionRepository
    {
        Task<AgentSession?> GetByIdAsync(int id);
        Task<AgentSession?> GetByConnectionIdAsync(string connectionId);
        Task<AgentSession?> GetActiveSessionByAgentIdAsync(int agentId);
        Task<IEnumerable<AgentSession>> GetActiveSessionsAsync();
        Task<int> AddAsync(AgentSession session);
        Task UpdateAsync(AgentSession session);
        Task EndExpiredSessionsAsync(TimeSpan timeout);
    }
}