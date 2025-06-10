// Application/Services/AgentService.cs
using AutoMapper;
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Application.Interfaces;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.Interfaces;

namespace CallCenterBilling.Application.Services
{
    public class AgentService : IAgentService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IMapper _mapper;
        
        public AgentService(IAgentRepository agentRepository, IMapper mapper)
        {
            _agentRepository = agentRepository;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<AgentDto>> GetAllAgentsAsync()
        {
            var agents = await _agentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AgentDto>>(agents);
        }
        
        public async Task<AgentDto?> GetAgentByIdAsync(int id)
        {
            var agent = await _agentRepository.GetByIdAsync(id);
            return agent != null ? _mapper.Map<AgentDto>(agent) : null;
        }
        
        public async Task<IEnumerable<AgentDto>> GetActiveAgentsAsync()
        {
            var agents = await _agentRepository.GetActiveAgentsAsync();
            return _mapper.Map<IEnumerable<AgentDto>>(agents);
        }
        
        public async Task<IEnumerable<AgentDto>> GetTopPerformingAgentsAsync(int count = 10)
        {
            var agents = await _agentRepository.GetTopPerformingAgentsAsync(count);
            return _mapper.Map<IEnumerable<AgentDto>>(agents);
        }
        
        public async Task<AgentDto> CreateAgentAsync(CreateAgentDto createAgentDto)
        {
            var agent = new Agent
            {
                Name = createAgentDto.Name,
                Email = createAgentDto.Email,
                PhoneNumber = createAgentDto.PhoneNumber,
                Status = AgentStatus.Offline,
                CreatedAt = DateTime.UtcNow,
                Rating = 0
            };
            
            var createdAgent = await _agentRepository.CreateAsync(agent);
            return _mapper.Map<AgentDto>(createdAgent);
        }
        
        public async Task<AgentDto> UpdateAgentAsync(int id, UpdateAgentDto updateAgentDto)
        {
            var agent = await _agentRepository.GetByIdAsync(id);
            if (agent == null)
                throw new ArgumentException($"Agent with ID {id} not found");
            
            agent.Name = updateAgentDto.Name;
            agent.Email = updateAgentDto.Email;
            agent.PhoneNumber = updateAgentDto.PhoneNumber;
            agent.UpdateRating(updateAgentDto.Rating);
            
            var updatedAgent = await _agentRepository.UpdateAsync(agent);
            return _mapper.Map<AgentDto>(updatedAgent);
        }
        
        public async Task<bool> DeleteAgentAsync(int id)
        {
            return await _agentRepository.DeleteAsync(id);
        }
        
        public async Task<bool> UpdateAgentStatusAsync(int id, AgentStatus status)
        {
            var agent = await _agentRepository.GetByIdAsync(id);
            if (agent == null)
                return false;
            
            switch (status)
            {
                case AgentStatus.Active:
                    agent.Activate();
                    break;
                case AgentStatus.OnBreak:
                    agent.SetOnBreak();
                    break;
                case AgentStatus.Offline:
                    agent.Deactivate();
                    break;
            }
            
            await _agentRepository.UpdateAsync(agent);
            return true;
        }
        
        public async Task<int> GetActiveAgentCountAsync()
        {
            return await _agentRepository.GetActiveAgentCountAsync();
        }
        
        public async Task<int> GetTotalAgentCountAsync()
        {
            return await _agentRepository.GetTotalAgentCountAsync();
        }
        
        public async Task<double> GetUtilizationRateAsync()
        {
            var totalAgents = await GetTotalAgentCountAsync();
            var activeAgents = await GetActiveAgentCountAsync();
            
            return totalAgents > 0 ? (double)activeAgents / totalAgents * 100 : 0;
        }
    }
}