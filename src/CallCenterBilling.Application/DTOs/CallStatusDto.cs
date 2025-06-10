// Application/DTOs/AgentDto.cs
using System.ComponentModel.DataAnnotations;
using CallCenterBilling.Domain.Entities;
using CallCenterBilling.Domain.ValueObjects;

namespace CallCenterBilling.Application.DTOs
{
    public record CallStatusDto
    {
        public int CallId { get; init; }
        public int AgentId { get; init; }
        public string AgentName { get; init; } = string.Empty;
        public string CustomerPhoneNumber { get; init; } = string.Empty;
        public string? CustomerName { get; init; }
        public CallStatus Status { get; init; }
        public DateTime StartTime { get; init; }
        public TimeSpan Duration { get; init; }
        public decimal? Revenue { get; init; }
    }
    public record AgentCallStatusDto
    {
        public int AgentId { get; init; }
        public string Name { get; init; } = string.Empty;
        public AgentStatus Status { get; init; }
        public bool IsOnCall { get; init; }
        public CallStatusDto? CurrentCall { get; init; }
        public DateTime LastActiveAt { get; init; }
        public TimeSpan? CurrentCallDuration { get; init; }
    }
    public record CallDashboardDto
    {
        public IEnumerable<AgentCallStatusDto> Agents { get; init; } = new List<AgentCallStatusDto>();
        public IEnumerable<CallStatusDto> ActiveCalls { get; init; } = new List<CallStatusDto>();
        public CallMetrics Metrics { get; init; } = new CallMetrics();
        public int OnlineAgents { get; init; }
        public int AgentsOnCall { get; init; }
        public DateTime LastUpdated { get; init; }
    }
}