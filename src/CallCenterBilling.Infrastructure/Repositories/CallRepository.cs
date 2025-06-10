using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Domain.ValueObjects;
using CallCenterBilling.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CallCenterBilling.Infrastructure.Repositories;

public class CallRepository : ICallRepository
{
    private readonly ApplicationDbContext _context;

    public CallRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Call?> GetByIdAsync(int id)
    {
        return await _context.Calls
            .Include(c => c.Agent)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Call>> GetActiveCallsAsync()
    {
        return await _context.Calls
            .Include(c => c.Agent)
            .Where(c => c.Status == CallStatus.InProgress)
            .ToListAsync();
    }

    public async Task<IEnumerable<Call>> GetCallsByAgentIdAsync(int agentId)
    {
        return await _context.Calls
            .Where(c => c.AgentId == agentId)
            .OrderByDescending(c => c.StartTime)
            .ToListAsync();
    }

    public async Task<Call?> GetCurrentCallByAgentIdAsync(int agentId)
    {
        return await _context.Calls
            .FirstOrDefaultAsync(c => c.AgentId == agentId && c.Status == CallStatus.InProgress);
    }

    public async Task<int> AddAsync(Call call)
    {
        _context.Calls.Add(call);
        await _context.SaveChangesAsync();
        return call.Id;
    }

    public async Task UpdateAsync(Call call)
    {
        _context.Calls.Update(call);
        await _context.SaveChangesAsync();
    }

    public async Task<CallMetrics> GetCallMetricsAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _context.Calls.AsQueryable();

        if (from.HasValue)
            query = query.Where(c => c.StartTime >= from.Value);
        
        if (to.HasValue)
            query = query.Where(c => c.StartTime <= to.Value);

        var totalActiveCalls = await query.CountAsync(c => c.Status == CallStatus.InProgress);
        var totalCompletedCalls = await query.CountAsync(c => c.Status == CallStatus.Completed);
        var totalFailedCalls = await query.CountAsync(c => c.Status == CallStatus.Failed);
        var totalRevenue = await query.Where(c => c.Revenue.HasValue).SumAsync(c => c.Revenue.Value);
        
        var completedCalls = await query.Where(c => c.Status == CallStatus.Completed && c.Duration > 0).ToListAsync();
        var averageDuration = completedCalls.Any() 
            ? TimeSpan.FromSeconds(completedCalls.Average(c => c.Duration))
            : TimeSpan.Zero;

        return new CallMetrics
        {
            TotalActiveCalls = totalActiveCalls,
            TotalCompletedCalls = totalCompletedCalls,
            TotalFailedCalls = totalFailedCalls,
            TotalRevenue = totalRevenue,
            AverageCallDuration = averageDuration,
            LastUpdated = DateTime.UtcNow
        };
    }
}
