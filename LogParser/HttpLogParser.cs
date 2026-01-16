using LogParser.Analysis;
using LogParser.Formatters;
using LogParser.Interfaces;
using LogParser.Models;
using LogParser.Parsers;

namespace LogParser
{
    /// <summary>
    /// Facade class that provides a simple API for parsing HTTP log files.
    /// Coordinates between the parser, analyzer, and formatter components.
    /// Maintains backward compatibility while using the new SRP-compliant classes internally.
    /// </summary>
    public class HttpLogParser
    {
        private readonly ILogLineParser _lineParser;
        private readonly ILogAnalyzer _analyzer;
        private readonly IReportFormatter _formatter;

        /// <summary>
        /// Creates a new HttpLogParser with default components (Common Log Format parser, text formatter).
        /// </summary>
        public HttpLogParser()
            : this(new CommonLogFormatParser(), new LogAnalyzer(), new TextReportFormatter())
        {
        }

        /// <summary>
        /// Creates a new HttpLogParser with custom components for extensibility.
        /// </summary>
        /// <param name="lineParser">Parser for individual log lines</param>
        /// <param name="analyzer">Analyzer for aggregating statistics</param>
        /// <param name="formatter">Formatter for output generation</param>
        public HttpLogParser(ILogLineParser lineParser, ILogAnalyzer analyzer, IReportFormatter formatter)
        {
            _lineParser = lineParser;
            _analyzer = analyzer;
            _formatter = formatter;
        }

        /// <summary>
        /// Parses a log file and generates a formatted report.
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        /// <param name="topN">Number of top items to include (default: 3)</param>
        /// <returns>Formatted report string</returns>
        public string ParseAndGenerateReport(string filePath, int topN = 3)
        {
            var report = ParseLogFileStreaming(filePath, topN);
            return _formatter.Format(report);
        }

        /// <summary>
        /// Parses a log file using streaming and returns aggregated statistics.
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        /// <param name="topN">Number of top items to include (default: 3)</param>
        /// <returns>LogReport with aggregated statistics</returns>
        public LogReport ParseLogFileStreaming(string filePath, int topN = 3)
        {
            return _analyzer.Analyze(filePath, _lineParser, topN);
        }
    }
}
