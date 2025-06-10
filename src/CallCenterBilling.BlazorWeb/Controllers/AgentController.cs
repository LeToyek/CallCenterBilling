// Controllers/AgentsController.cs
using Microsoft.AspNetCore.Mvc;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Application.DTOs;
using CallCenterBilling.Application.Interfaces;


namespace CallCenter.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _agentService;
        
        public AgentsController(IAgentService agentService)
        {
            _agentService = agentService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgentDto>>> GetAllAgents()
        {
            var agents = await _agentService.GetAllAgentsAsync();
            return Ok(agents);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<AgentDto>> GetAgent(int id)
        {
            var agent = await _agentService.GetAgentByIdAsync(id);
            if (agent == null)
                return NotFound();
            
            return Ok(agent);
        }
        
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AgentDto>>> GetActiveAgents()
        {
            var agents = await _agentService.GetActiveAgentsAsync();
            return Ok(agents);
        }
        
        [HttpGet("top-performing")]
        public async Task<ActionResult<IEnumerable<AgentDto>>> GetTopPerformingAgents([FromQuery] int count = 10)
        {
            var agents = await _agentService.GetTopPerformingAgentsAsync(count);
            return Ok(agents);
        }
        
        [HttpPost]
        public async Task<ActionResult<AgentDto>> CreateAgent(CreateAgentDto createAgentDto)
        {
            try
            {
                var agent = await _agentService.CreateAgentAsync(createAgentDto);
                return CreatedAtAction(nameof(GetAgent), new { id = agent.Id }, agent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<AgentDto>> UpdateAgent(int id, UpdateAgentDto updateAgentDto)
        {
            try
            {
                var agent = await _agentService.UpdateAgentAsync(id, updateAgentDto);
                return Ok(agent);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateAgentStatus(int id, [FromBody] AgentStatusDto statusDto)
        {
            if (!Enum.TryParse<AgentStatus>(statusDto.Status, out var status))
                return BadRequest("Invalid status");
            
            var result = await _agentService.UpdateAgentStatusAsync(id, status);
            if (!result)
                return NotFound();
            
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAgent(int id)
        {
            var result = await _agentService.DeleteAgentAsync(id);
            if (!result)
                return NotFound();
            
            return NoContent();
        }
        
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetAgentStats()
        {
            var activeCount = await _agentService.GetActiveAgentCountAsync();
            var totalCount = await _agentService.GetTotalAgentCountAsync();
            var utilizationRate = await _agentService.GetUtilizationRateAsync();
            
            return Ok(new
            {
                ActiveAgents = activeCount,
                TotalAgents = totalCount,
                UtilizationRate = utilizationRate
            });
        }
    }
}