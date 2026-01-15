namespace LogParser
{
    /// <summary>
    /// Represents a single parsed HTTP log entry
    /// </summary>
    public class LogEntry
    {
        public string IpAddress { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public string HttpMethod { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
        public int StatusCode { get; init; }
        public int ResponseSize { get; init; }
    }
}
