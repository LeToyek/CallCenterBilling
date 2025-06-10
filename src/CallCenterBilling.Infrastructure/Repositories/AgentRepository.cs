using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;
using CallCenterBilling.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CallCenterBilling.Infrastructure.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly ApplicationDbContext _context;
        
        public AgentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Agent>> GetAllAsync()
        {
            return await _context.Agents
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
        
        public async Task<Agent?> GetByIdAsync(int id)
        {
            return await _context.Agents.FindAsync(id);
        }
        
        public async Task<IEnumerable<Agent>> GetActiveAgentsAsync()
        {
            return await _context.Agents
                .Where(a => a.Status == AgentStatus.Active)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Agent>> GetTopPerformingAgentsAsync(int count = 10)
        {
            return await _context.Agents
                .OrderByDescending(a => a.TotalRevenue)
                .ThenByDescending(a => a.Rating)
                .Take(count)
                .ToListAsync();
        }
        
        public async Task<Agent> CreateAsync(Agent agent)
        {
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();
            return agent;
        }
        
        public async Task<Agent> UpdateAsync(Agent agent)
        {
            _context.Agents.Update(agent);
            await _context.SaveChangesAsync();
            return agent;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
                return false;
            
            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<int> GetActiveAgentCountAsync()
        {
            return await _context.Agents
                .CountAsync(a => a.Status == AgentStatus.Active);
        }
        
        public async Task<int> GetTotalAgentCountAsync()
        {
            return await _context.Agents.CountAsync();
        }
        
        public async Task<double> GetAverageRatingAsync()
        {
            return await _context.Agents
                .Where(a => a.Rating > 0)
                .AverageAsync(a => a.Rating);
        }
    }
}