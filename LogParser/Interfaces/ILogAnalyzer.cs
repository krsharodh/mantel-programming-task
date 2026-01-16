using LogParser.Models;

namespace LogParser.Interfaces
{
    /// <summary>
    /// Interface for analyzing log entries and aggregating statistics.
    /// Implement this interface to support different aggregation strategies.
    /// </summary>
    public interface ILogAnalyzer
    {
        /// <summary>
        /// Analyzes a log file and returns aggregated statistics.
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        /// <param name="parser">Parser to use for each line</param>
        /// <param name="topN">Number of top items to include</param>
        /// <returns>LogReport with aggregated statistics</returns>
        LogReport Analyze(string filePath, ILogLineParser parser, int topN = 3);

        /// <summary>
        /// Analyzes log lines from any enumerable source.
        /// </summary>
        /// <param name="lines">Enumerable of log lines</param>
        /// <param name="parser">Parser to use for each line</param>
        /// <param name="topN">Number of top items to include</param>
        /// <returns>LogReport with aggregated statistics</returns>
        LogReport Analyze(IEnumerable<string> lines, ILogLineParser parser, int topN = 3);
    }
}
