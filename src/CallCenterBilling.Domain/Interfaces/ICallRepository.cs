using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.ValueObjects;

namespace CallCenterBilling.Domain.Interfaces
{
    public interface ICallRepository
    {
        Task<Call?> GetByIdAsync(int id);
        Task<IEnumerable<Call>> GetActiveCallsAsync();
        Task<IEnumerable<Call>> GetCallsByAgentIdAsync(int agentId);
        Task<Call?> GetCurrentCallByAgentIdAsync(int agentId);
        Task<int> AddAsync(Call call);
        Task UpdateAsync(Call call);
        Task<CallMetrics> GetCallMetricsAsync(DateTime? from = null, DateTime? to = null);
    }
}