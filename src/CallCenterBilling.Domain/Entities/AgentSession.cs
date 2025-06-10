
namespace CallCenterBilling.Domain.Entities
{

    public class AgentSession
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string ConnectionId { get; set; } = string.Empty; // SignalR connection ID
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public AgentSessionStatus Status { get; set; } = AgentSessionStatus.Active;

        // Navigation properties
        public Agent Agent { get; set; } = null!;

        // Domain methods
        public void UpdateHeartbeat()
        {
            LastHeartbeat = DateTime.UtcNow;
        }

        public void Logout()
        {
            LogoutTime = DateTime.UtcNow;
            Status = AgentSessionStatus.Ended;
        }

        public bool IsExpired(TimeSpan timeout)
        {
            return DateTime.UtcNow - LastHeartbeat > timeout;
        }

        public TimeSpan SessionDuration => LogoutTime.HasValue
            ? LogoutTime.Value - LoginTime
            : DateTime.UtcNow - LoginTime;
    }
}