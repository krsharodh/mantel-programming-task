namespace LogParser.Models
{
    /// <summary>
    /// Represents a single parsed log entry from an HTTP access log
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// The IP address of the client making the request
        /// </summary>
        public required string IpAddress { get; init; }

        /// <summary>
        /// The HTTP method used (GET, POST, etc.)
        /// </summary>
        public required string HttpMethod { get; init; }

        /// <summary>
        /// The URL/path that was requested
        /// </summary>
        public required string Url { get; init; }

        /// <summary>
        /// The timestamp of the request
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// The HTTP status code returned
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// The size of the response in bytes
        /// </summary>
        public int ResponseSize { get; init; }
    }
}
