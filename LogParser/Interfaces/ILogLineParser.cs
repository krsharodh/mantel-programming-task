using LogParser.Models;

namespace LogParser.Interfaces
{
    /// <summary>
    /// Interface for parsing individual log lines.
    /// Implement this interface to support different log formats.
    /// </summary>
    public interface ILogLineParser
    {
        /// <summary>
        /// Parses a single log line into a LogEntry.
        /// </summary>
        /// <param name="line">The raw log line to parse</param>
        /// <returns>A LogEntry if parsing succeeds, null otherwise</returns>
        LogEntry? Parse(string line);
    }
}
