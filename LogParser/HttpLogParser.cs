using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LogParser
{
    /// <summary>
    /// Parses HTTP access log files in Common Log Format / Combined Log Format
    /// </summary>
    public class HttpLogParser
    {
        // Regex pattern to parse Common/Combined Log Format
        // Example: 177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] "GET /intranet-analytics/ HTTP/1.1" 200 3574
        private static readonly Regex LogPattern = new Regex(
            @"^(?<ip>\S+)\s+" +                           // IP address
            @"\S+\s+" +                                    // Identity (usually -)
            @"\S+\s+" +                                    // User (usually -)
            @"\[(?<timestamp>[^\]]+)\]\s+" +               // Timestamp
            @"""(?<method>\S+)\s+(?<url>\S+)\s+\S+""\s+" + // Request line (method, URL, protocol)
            @"(?<status>\d+)\s+" +                         // Status code
            @"(?<size>\S+)",                               // Response size
            RegexOptions.Compiled);

        /// <summary>
        /// Parses a log file and generates a report
        /// </summary>
        public string ParseAndGenerateReport(string filePath, int topN = 3)
        {
            var report = ParseLogFileStreaming(filePath, topN);
            return FormatReport(report);
        }

        /// <summary>
        /// Parses a log file using streaming and aggregates data on-the-fly
        /// </summary>
        public LogReport ParseLogFileStreaming(string filePath, int topN = 3)
        {
            var ipCounts = new Dictionary<string, int>();
            var urlCounts = new Dictionary<string, int>();

            foreach (var line in File.ReadLines(filePath))
            {
                var entry = ParseLogLine(line);
                if (entry == null) continue;

                // Aggregate IP counts on-the-fly
                ipCounts[entry.IpAddress] = ipCounts.GetValueOrDefault(entry.IpAddress) + 1;

                // Aggregate URL counts on-the-fly
                urlCounts[entry.Url] = urlCounts.GetValueOrDefault(entry.Url) + 1;
            }

            return new LogReport
            {
                UniqueIpCount = ipCounts.Count,
                TopUrls = urlCounts
                    .OrderByDescending(x => x.Value)
                    .ThenBy(x => x.Key)
                    .Take(topN)
                    .Select(x => (x.Key, x.Value))
                    .ToList(),
                TopIpAddresses = ipCounts
                    .OrderByDescending(x => x.Value)
                    .ThenBy(x => x.Key)
                    .Take(topN)
                    .Select(x => (x.Key, x.Value))
                    .ToList()
            };
        }

        /// <summary>
        /// Parses a single log line into a LogEntry object
        /// </summary>
        public LogEntry? ParseLogLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var match = LogPattern.Match(line);
            if (!match.Success)
                return null;

            var entry = new LogEntry
            {
                IpAddress = match.Groups["ip"].Value,
                HttpMethod = match.Groups["method"].Value,
                Url = match.Groups["url"].Value,
                Timestamp = DateTime.TryParseExact(
                    match.Groups["timestamp"].Value,
                    "dd/MMM/yyyy:HH:mm:ss zzz",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime dateTime) ? dateTime : DateTime.MinValue,
                StatusCode = int.TryParse(match.Groups["status"].Value, out int status) ? status : 0,
                ResponseSize = int.TryParse(match.Groups["size"].Value, out int size) ? size : 0
            };

            return entry;
        }

        /// <summary>
        /// Formats a LogReport into a readable string
        /// </summary>
        public string FormatReport(LogReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine("HTTP LOG ANALYSIS REPORT");
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine();

            // 1. Number of unique IP addresses
            sb.AppendLine($"Number of Unique IP Addresses: {report.UniqueIpCount}");
            sb.AppendLine();

            // 2. Top 3 most visited URLs
            sb.AppendLine("Top 3 Most Visited URLs:");
            sb.AppendLine("-".PadRight(30, '-'));
            int rank = 1;
            foreach (var (url, count) in report.TopUrls)
            {
                sb.AppendLine($"  {rank}. {url} ({count} visits)");
                rank++;
            }
            sb.AppendLine();

            // 3. Top 3 most active IP addresses
            sb.AppendLine("Top 3 Most Active IP Addresses:");
            sb.AppendLine("-".PadRight(30, '-'));
            rank = 1;
            foreach (var (ip, count) in report.TopIpAddresses)
            {
                sb.AppendLine($"  {rank}. {ip} ({count} requests)");
                rank++;
            }
            sb.AppendLine();
            sb.AppendLine("=".PadRight(50, '='));

            return sb.ToString();
        }
    }
}
