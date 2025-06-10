// Application/Services/IAgentService.cs
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Domain.Entities;

namespace CallCenterBilling.Application.Interfaces
{
    public interface IAgentService
    {
        Task<IEnumerable<AgentDto>> GetAllAgentsAsync();
        Task<AgentDto?> GetAgentByIdAsync(int id);
        Task<IEnumerable<AgentDto>> GetActiveAgentsAsync();
        Task<IEnumerable<AgentDto>> GetTopPerformingAgentsAsync(int count = 10);
        Task<AgentDto> CreateAgentAsync(CreateAgentDto createAgentDto);
        Task<AgentDto> UpdateAgentAsync(int id, UpdateAgentDto updateAgentDto);
        Task<bool> DeleteAgentAsync(int id);
        Task<bool> UpdateAgentStatusAsync(int id, AgentStatus status);
        Task<int> GetActiveAgentCountAsync();
        Task<int> GetTotalAgentCountAsync();
        Task<double> GetUtilizationRateAsync();
    }
}