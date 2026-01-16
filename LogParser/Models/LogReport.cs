namespace LogParser.Models
{
    /// <summary>
    /// Represents the aggregated report from parsing log files
    /// </summary>
    public class LogReport
    {
        /// <summary>
        /// Total count of unique IP addresses found in the log
        /// </summary>
        public int UniqueIpCount { get; init; }

        /// <summary>
        /// List of top URLs with their visit counts, ordered by count descending
        /// </summary>
        public required List<(string Url, int Count)> TopUrls { get; init; }

        /// <summary>
        /// List of top IP addresses with their request counts, ordered by count descending
        /// </summary>
        public required List<(string IpAddress, int Count)> TopIpAddresses { get; init; }
    }
}
