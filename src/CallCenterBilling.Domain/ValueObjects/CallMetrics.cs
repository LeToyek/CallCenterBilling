namespace CallCenterBilling.Domain.ValueObjects
{
    public record CallMetrics
    {
        public int TotalActiveCalls { get; init; }
        public int TotalCompletedCalls { get; init; }
        public int TotalFailedCalls { get; init; }
        public decimal TotalRevenue { get; init; }
        public TimeSpan AverageCallDuration { get; init; }
        public DateTime LastUpdated { get; init; }
    }

}