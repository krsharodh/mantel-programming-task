using System.Text;
using LogParser.Interfaces;
using LogParser.Models;

namespace LogParser.Formatters
{
    /// <summary>
    /// Formats log reports as human-readable plain text.
    /// Single responsibility: Convert LogReport to text output.
    /// </summary>
    public class TextReportFormatter : IReportFormatter
    {
        /// <summary>
        /// Formats a LogReport into a readable text string.
        /// </summary>
        /// <param name="report">The report to format</param>
        /// <returns>Formatted plain text representation</returns>
        public string Format(LogReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine("HTTP LOG ANALYSIS REPORT");
            sb.AppendLine("=".PadRight(50, '='));
            sb.AppendLine();

            // 1. Number of unique IP addresses
            sb.AppendLine($"Number of Unique IP Addresses: {report.UniqueIpCount}");
            sb.AppendLine();

            // 2. Top most visited URLs
            sb.AppendLine($"Top {report.TopUrls.Count} Most Visited URLs:");
            sb.AppendLine("-".PadRight(30, '-'));
            int rank = 1;
            foreach (var (url, count) in report.TopUrls)
            {
                sb.AppendLine($"  {rank}. {url} ({count} visits)");
                rank++;
            }
            sb.AppendLine();

            // 3. Top most active IP addresses
            sb.AppendLine($"Top {report.TopIpAddresses.Count} Most Active IP Addresses:");
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
