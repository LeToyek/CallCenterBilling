
// Target framework: .NET 8.0
using System;
using CallCenterBilling.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CallCenterBilling.Domain.Entities
{
    public class Call
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public CallStatus Status { get; set; } = CallStatus.Initiated;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Duration { get; set; } // in seconds
        public decimal? Revenue { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Agent Agent { get; set; } = null!;

        // Domain methods
        public void Start()
        {
            Status = CallStatus.InProgress;
            StartTime = DateTime.UtcNow;
        }

        public void End(string? notes = null)
        {
            Status = CallStatus.Completed;
            EndTime = DateTime.UtcNow;
            Duration = (int)(EndTime.Value - StartTime).TotalSeconds;
            Notes = notes;
        }

        public void Fail(string? reason = null)
        {
            Status = CallStatus.Failed;
            EndTime = DateTime.UtcNow;
            Duration = (int)(EndTime.Value - StartTime).TotalSeconds;
            Notes = reason;
        }

        public bool IsActive => Status == CallStatus.InProgress;
        public TimeSpan CallDuration => EndTime.HasValue
            ? EndTime.Value - StartTime
            : DateTime.UtcNow - StartTime;
    }
}