using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Domain.ValueObjects;
using CallCenterBilling.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CallCenterBilling.Infrastructure.Repositories;

public class AgentSessionRepository : IAgentSessionRepository
{
    private readonly ApplicationDbContext _context;

    public AgentSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AgentSession?> GetByIdAsync(int id)
    {
        return await _context.AgentSessions
            .Include(s => s.Agent)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<AgentSession?> GetByConnectionIdAsync(string connectionId)
    {
        return await _context.AgentSessions
            .Include(s => s.Agent)
            .FirstOrDefaultAsync(s => s.ConnectionId == connectionId && s.Status == AgentSessionStatus.Active);
    }

    public async Task<AgentSession?> GetActiveSessionByAgentIdAsync(int agentId)
    {
        return await _context.AgentSessions
            .FirstOrDefaultAsync(s => s.AgentId == agentId && s.Status == AgentSessionStatus.Active);
    }

    public async Task<IEnumerable<AgentSession>> GetActiveSessionsAsync()
    {
        return await _context.AgentSessions
            .Include(s => s.Agent)
            .Where(s => s.Status == AgentSessionStatus.Active)
            .ToListAsync();
    }

    public async Task<int> AddAsync(AgentSession session)
    {
        _context.AgentSessions.Add(session);
        await _context.SaveChangesAsync();
        return session.Id;
    }

    public async Task UpdateAsync(AgentSession session)
    {
        _context.AgentSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task EndExpiredSessionsAsync(TimeSpan timeout)
    {
        // 1. Calculate the cutoff time *before* the query. 
        // This creates a simple variable that EF Core can easily use as a parameter.
        var cutoffTime = DateTime.UtcNow - timeout;

        // 2. Use the simple variable in the WHERE clause. This is easily translated to SQL.
        var expiredSessions = await _context.AgentSessions
            .Where(s => s.Status == AgentSessionStatus.Active && s.LastHeartbeat < cutoffTime)
            .ToListAsync();

        foreach (var session in expiredSessions)
        {
            // This part of your logic is correct and remains the same.
            session.Logout();
        }

        await _context.SaveChangesAsync();
    }
}
