# HTTP Log Parser

A C# application that parses HTTP access log files and generates reports on:
- Number of unique IP addresses
- Top 3 most visited URLs
- Top 3 most active IP addresses

## Requirements

- .NET 9.0 SDK

## Project Structure

```
├── LogParser/                          # Main application
│   ├── LogParser.csproj
│   ├── Program.cs                      # Entry point
│   ├── HttpLogParser.cs                # Facade coordinating parsing components
│   ├── sample.log                      # Sample log file for testing
│   ├── Interfaces/                     # Contracts for extensibility
│   │   ├── ILogLineParser.cs           # Interface for log format parsers
│   │   ├── ILogAnalyzer.cs             # Interface for log analyzers
│   │   └── IReportFormatter.cs         # Interface for report formatters
│   ├── Models/                         # Data transfer objects
│   │   ├── LogEntry.cs                 # Data model for a single log entry
│   │   └── LogReport.cs                # Data model for aggregated report
│   ├── Parsers/                        # Log format implementations
│   │   └── CommonLogFormatParser.cs    # Parser for Common/Combined Log Format
│   ├── Analysis/                       # Business logic
│   │   └── LogAnalyzer.cs              # Aggregation logic (streaming)
│   └── Formatters/                     # Output format implementations
│       └── TextReportFormatter.cs      # Plain text output formatter
└── LogParser.Tests/                    # Unit tests
    ├── LogParser.Tests.csproj
    ├── HttpLogParserIntegrationTests.cs  # Integration tests for facade
    ├── Parsers/
    │   └── CommonLogFormatParserTests.cs
    ├── Analysis/
    │   └── LogAnalyzerTests.cs
    └── Formatters/
        └── TextReportFormatterTests.cs
```

## Building

```bash
dotnet build
```

## Running

```bash
cd LogParser
dotnet run -- <path-to-log-file>
```

Example:
```bash
dotnet run -- programming-task-example-data.log
```

## Sample Output

```
==================================================
HTTP LOG ANALYSIS REPORT
==================================================

Number of Unique IP Addresses: 5

Top 3 Most Visited URLs:
------------------------------
  1. /download/counter/ (4 visits)
  2. /blog/2018/08/survey-b/ (2 visits)
  3. /hosting/ (2 visits)

Top 3 Most Active IP Addresses:
------------------------------
  1. 72.44.32.10 (4 requests)
  2. 168.41.191.40 (3 requests)
  3. 177.71.128.21 (3 requests)

==================================================
```

## Running Tests

```bash
dotnet test
```

## Design Decisions & Assumptions

### Log Format Assumptions
- Input files are in **Common Log Format (CLF)** or **Combined Log Format**
- Each log line follows the pattern: `IP - - [timestamp] "METHOD URL PROTOCOL" STATUS SIZE`
- Malformed lines are silently skipped (logged parsing errors could be added)
- The parser extracts the URL path from the request line, not including query strings

### Memory Efficiency (Streaming Approach)
- Uses `File.ReadLines()` for lazy, line-by-line reading instead of loading the entire file
- Memory usage is **O(unique items)** not O(total lines) - only unique IPs and URLs are stored
- Suitable for processing very large log files (gigabytes) on memory-constrained systems

### Architecture (Single Responsibility Principle)
The codebase is organized into focused, single-responsibility components:

| Component | Responsibility |
|-----------|----------------|
| `ILogLineParser` | Interface for parsing individual log lines |
| `CommonLogFormatParser` | Parses Common/Combined Log Format lines |
| `ILogAnalyzer` | Interface for aggregating statistics |
| `LogAnalyzer` | Aggregates statistics from parsed entries |
| `IReportFormatter` | Interface for formatting output |
| `TextReportFormatter` | Formats reports as plain text |
| `HttpLogParser` | Facade coordinating all components |

### Extensibility
The interface-based design allows easy extension:
- **New log formats**: Implement `ILogLineParser` (e.g., for W3C, IIS, or custom formats)
- **New analyzers**: Implement `ILogAnalyzer` (e.g., parallel processing, different aggregation strategies)
- **New output formats**: Implement `IReportFormatter` (e.g., JSON, CSV, XML, HTML)
- **Dependency Injection**: The `HttpLogParser` facade accepts components via constructor

Example: Using a custom formatter
```csharp
var parser = new HttpLogParser(
    new CommonLogFormatParser(),
    new LogAnalyzer(),
    new MyCustomFormatter()  // Implement IReportFormatter
);
```

### Top N Ties
When multiple items have the same count, they are sorted alphabetically as a secondary sort key to ensure deterministic output.

## Supported Log Format

The parser supports the Common Log Format / Combined Log Format used by Apache and Nginx:

```
177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] "GET /intranet-analytics/ HTTP/1.1" 200 3574
```

Format breakdown:
- IP address
- Identity (typically `-`)
- User (typically `-`)
- Timestamp in brackets (format: `dd/MMM/yyyy:HH:mm:ss zzz`)
- Request line (method, URL, protocol) in quotes
- HTTP status code
- Response size in bytes