using LogParser.Interfaces;
using LogParser.Models;

namespace LogParser.Analysis
{
    /// <summary>
    /// Analyzes log entries and aggregates statistics.
    /// Single responsibility: Aggregate data from parsed log entries.
    /// Uses streaming approach for memory efficiency.
    /// </summary>
    public class LogAnalyzer : ILogAnalyzer
    {
        /// <summary>
        /// Analyzes a log file using streaming and returns aggregated statistics.
        /// Memory usage is O(unique items) not O(total lines).
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        /// <param name="parser">Parser to use for each line</param>
        /// <param name="topN">Number of top items to include</param>
        /// <returns>LogReport with aggregated statistics</returns>
        public LogReport Analyze(string filePath, ILogLineParser parser, int topN = 3)
        {
            return Analyze(File.ReadLines(filePath), parser, topN);
        }

        /// <summary>
        /// Analyzes log lines from any enumerable source (useful for testing).
        /// </summary>
        /// <param name="lines">Enumerable of log lines</param>
        /// <param name="parser">Parser to use for each line</param>
        /// <param name="topN">Number of top items to include</param>
        /// <returns>LogReport with aggregated statistics</returns>
        public LogReport Analyze(IEnumerable<string> lines, ILogLineParser parser, int topN = 3)
        {
            var ipCounts = new Dictionary<string, int>();
            var urlCounts = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                var entry = parser.Parse(line);
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
    }
}
