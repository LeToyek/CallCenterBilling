using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.ValueObjects;

namespace CallCenterBilling.Application.Interfaces;
public interface ICallService
{
    Task<int> StartCallAsync(int agentId, string customerPhoneNumber, string? customerName = null);
    Task EndCallAsync(int callId, decimal? revenue = null, string? notes = null);
    Task FailCallAsync(int callId, string? reason = null);
    Task<CallStatusDto?> GetCallStatusAsync(int callId);
    Task<IEnumerable<CallStatusDto>> GetActiveCallsAsync();
    Task<CallMetrics> GetCallMetricsAsync(DateTime? from = null, DateTime? to = null);
}
