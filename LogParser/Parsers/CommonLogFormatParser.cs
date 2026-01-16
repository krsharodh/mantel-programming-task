using System.Globalization;
using System.Text.RegularExpressions;
using LogParser.Interfaces;
using LogParser.Models;

namespace LogParser.Parsers
{
    /// <summary>
    /// Parser for Common Log Format / Combined Log Format used by Apache and Nginx.
    /// Single responsibility: Parse individual log lines into LogEntry objects.
    /// </summary>
    public class CommonLogFormatParser : ILogLineParser
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
        /// Parses a single log line into a LogEntry object.
        /// </summary>
        /// <param name="line">The raw log line to parse</param>
        /// <returns>A LogEntry if parsing succeeds, null otherwise</returns>
        public LogEntry? Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var match = LogPattern.Match(line);
            if (!match.Success)
                return null;

            return new LogEntry
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
        }
    }
}
