namespace LogParser
{
    /// <summary>
    /// Represents the aggregated report from parsing log files
    /// </summary>
    public class LogReport
    {
        public int UniqueIpCount { get; init; }
        public List<(string Url, int Count)> TopUrls { get; init; } = [];
        public List<(string IpAddress, int Count)> TopIpAddresses { get; init; } = [];
    }
}
