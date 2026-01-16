# HTTP Log Parser

A C# application that parses HTTP access log files and generates reports on:
- Number of unique IP addresses
- Top 3 most visited URLs
- Top 3 most active IP addresses

## Requirements

- .NET 9.0 SDK

## Project Structure

```
├── LogParser/                    # Main application
│   ├── LogParser.csproj
│   ├── Program.cs               # Entry point
│   ├── HttpLogParser.cs         # Core parsing logic
│   ├── LogEntry.cs              # Data model
│   └── sample.log               # Sample log file for testing
└── LogParser.Tests/             # Unit tests
    ├── LogParser.Tests.csproj
    └── HttpLogParserTests.cs
```

## Building

```bash
cd LogParser
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
cd LogParser.Tests
dotnet run -- programming-task-example-data.log
```

## Supported Log Format

The parser supports the Common Log Format / Combined Log Format used by Apache and Nginx:

```
177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] "GET /intranet-analytics/ HTTP/1.1" 200 3574
```

Format breakdown:
- IP address
- Identity (typically `-`)
- User (typically `-`)
- Timestamp in brackets
- Request line (method, URL, protocol) in quotes
- HTTP status code
- Response size in bytes