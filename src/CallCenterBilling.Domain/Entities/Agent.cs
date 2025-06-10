using System.ComponentModel.DataAnnotations;

namespace CallCenterBilling.Domain.Entities
{
    public class Agent
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        public AgentStatus Status { get; set; } = AgentStatus.Offline;
        
        public int TotalCalls { get; set; }
        
        public decimal TotalRevenue { get; set; }
        
        public double Rating { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? LastActiveAt { get; set; }
        
        // Domain methods
        public void Activate()
        {
            Status = AgentStatus.Active;
            LastActiveAt = DateTime.UtcNow;
        }
        
        public void Deactivate()
        {
            Status = AgentStatus.Offline;
        }
        
        public void SetOnBreak()
        {
            Status = AgentStatus.OnBreak;
        }
        
        public void UpdatePerformance(int additionalCalls, decimal additionalRevenue)
        {
            TotalCalls += additionalCalls;
            TotalRevenue += additionalRevenue;
        }
        
        public void UpdateRating(double newRating)
        {
            if (newRating >= 1 && newRating <= 5)
            {
                Rating = newRating;
            }
        }
        
        public bool IsActive => Status == AgentStatus.Active;
    }
}