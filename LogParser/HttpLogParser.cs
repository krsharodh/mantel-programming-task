using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public string ParseAndGenerateReport(string filePath)
        {
            var entries = ParseLogFile(filePath);
            return GenerateReport(entries);
        }

        /// <summary>
        /// Parses all entries from a log file
        /// </summary>
        public List<LogEntry> ParseLogFile(string filePath)
        {
            var entries = new List<LogEntry>();

            foreach (var line in File.ReadLines(filePath))
            {
                var entry = ParseLogLine(line);
                if (entry != null)
                {
                    entries.Add(entry);
                }
            }

            return entries;
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

            int.TryParse(match.Groups["size"].Value, out int size);

            var entry = new LogEntry
            {
                IpAddress = match.Groups["ip"].Value,
                HttpMethod = match.Groups["method"].Value,
                Url = match.Groups["url"].Value,
                StatusCode = int.TryParse(match.Groups["status"].Value, out int status) ? status : -1,
                ResponseSize = size
            };

            return entry;
        }

        /// <summary>
        /// Generates a report from parsed log entries
        /// </summary>
        public string GenerateReport(List<LogEntry> entries)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine("HTTP LOG ANALYSIS REPORT");
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine();

            // 1. Number of unique IP addresses
            var uniqueIpCount = GetUniqueIpAddressCount(entries);
            sb.AppendLine($"Number of Unique IP Addresses: {uniqueIpCount}");
            sb.AppendLine();

            // 2. Top 3 most visited URLs
            sb.AppendLine("Top 3 Most Visited URLs:");
            sb.AppendLine("-".PadRight(30, '-'));
            var topUrls = GetTopVisitedUrls(entries, 3);
            int rank = 1;
            foreach (var (url, count) in topUrls)
            {
                sb.AppendLine($"  {rank}. {url} ({count} visits)");
                rank++;
            }
            sb.AppendLine();

            // 3. Top 3 most active IP addresses
            sb.AppendLine("Top 3 Most Active IP Addresses:");
            sb.AppendLine("-".PadRight(30, '-'));
            var topIps = GetTopActiveIpAddresses(entries, 3);
            rank = 1;
            foreach (var (ip, count) in topIps)
            {
                sb.AppendLine($"  {rank}. {ip} ({count} requests)");
                rank++;
            }
            sb.AppendLine();
            sb.AppendLine("=".PadRight(50, '='));

            return sb.ToString();
        }

        /// <summary>
        /// Gets the count of unique IP addresses
        /// </summary>
        public int GetUniqueIpAddressCount(List<LogEntry> entries)
        {
            return entries.Select(e => e.IpAddress).Distinct().Count();
        }

        /// <summary>
        /// Gets the top N most visited URLs
        /// </summary>
        public List<(string Url, int Count)> GetTopVisitedUrls(List<LogEntry> entries, int topN)
        {
            return entries
                .GroupBy(e => e.Url)
                .Select(g => (Url: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Url)
                .Take(topN)
                .ToList();
        }

        /// <summary>
        /// Gets the top N most active IP addresses
        /// </summary>
        public List<(string IpAddress, int Count)> GetTopActiveIpAddresses(List<LogEntry> entries, int topN)
        {
            return entries
                .GroupBy(e => e.IpAddress)
                .Select(g => (IpAddress: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.IpAddress)
                .Take(topN)
                .ToList();
        }
    }
}
