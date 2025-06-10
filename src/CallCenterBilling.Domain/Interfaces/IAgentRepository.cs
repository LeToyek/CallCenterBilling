using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Domain.Interfaces
{
    public interface IAgentRepository
    {
        Task<IEnumerable<Agent>> GetAllAsync();
        Task<Agent?> GetByIdAsync(int id);
        Task<IEnumerable<Agent>> GetActiveAgentsAsync();
        Task<IEnumerable<Agent>> GetTopPerformingAgentsAsync(int count = 10);
        Task<Agent> CreateAsync(Agent agent);
        Task<Agent> UpdateAsync(Agent agent);
        Task<bool> DeleteAsync(int id);
        Task<int> GetActiveAgentCountAsync();
        Task<int> GetTotalAgentCountAsync();
        Task<double> GetAverageRatingAsync();
    }
}