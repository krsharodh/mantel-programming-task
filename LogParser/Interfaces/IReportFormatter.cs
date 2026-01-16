using LogParser.Models;

namespace LogParser.Interfaces
{
    /// <summary>
    /// Interface for formatting log reports.
    /// Implement this interface to support different output formats.
    /// </summary>
    public interface IReportFormatter
    {
        /// <summary>
        /// Formats a LogReport into a string representation.
        /// </summary>
        /// <param name="report">The report to format</param>
        /// <returns>Formatted string representation</returns>
        string Format(LogReport report);
    }
}
