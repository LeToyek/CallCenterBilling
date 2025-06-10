// Application/DTOs/AgentDto.cs
using System.ComponentModel.DataAnnotations;

namespace CallCenterBilling.Application.DTOs
{
    public class AgentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalCalls { get; set; }
        public decimal TotalRevenue { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
    }
    
    public class CreateAgentDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string? PhoneNumber { get; set; }
    }
    
    public class UpdateAgentDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public double Rating { get; set; }
    }
    
    public class AgentStatusDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}