
using CallCenterBilling.Domain.Entities;
namespace CallCenterBilling.Domain.ValueObjects
{
    public record AgentCallStatus
    {
        public int AgentId { get; init; }
        public string AgentName { get; init; } = string.Empty;
        public AgentStatus Status { get; init; }
        public Call? CurrentCall { get; init; }
        public DateTime LastActiveAt { get; init; }
        public bool IsOnCall => CurrentCall?.IsActive == true;
        public TimeSpan? CurrentCallDuration => CurrentCall?.CallDuration;
    }


}