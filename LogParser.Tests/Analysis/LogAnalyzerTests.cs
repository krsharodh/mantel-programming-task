using LogParser.Analysis;
using LogParser.Parsers;
using Xunit;

namespace LogParser.Tests.Analysis
{
    public class LogAnalyzerTests
    {
        private readonly LogAnalyzer _analyzer;
        private readonly CommonLogFormatParser _parser;

        public LogAnalyzerTests()
        {
            _analyzer = new LogAnalyzer();
            _parser = new CommonLogFormatParser();
        }

        [Fact]
        public void Analyze_ValidLines_ReturnsCorrectUniqueIpCount()
        {
            // Arrange
            var lines = new[]
            {
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100",
                "192.168.1.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page4/ HTTP/1.1\" 200 100"
            };

            // Act
            var report = _analyzer.Analyze(lines, _parser);

            // Assert
            Assert.Equal(3, report.UniqueIpCount);
        }

        [Fact]
        public void Analyze_ValidLines_ReturnsTopUrls()
        {
            // Arrange
            var lines = new[]
            {
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100"
            };

            // Act
            var report = _analyzer.Analyze(lines, _parser, topN: 3);

            // Assert
            Assert.Equal(3, report.TopUrls.Count);
            Assert.Equal("/page1/", report.TopUrls[0].Url);
            Assert.Equal(3, report.TopUrls[0].Count);
            Assert.Equal("/page2/", report.TopUrls[1].Url);
            Assert.Equal(2, report.TopUrls[1].Count);
            Assert.Equal("/page3/", report.TopUrls[2].Url);
            Assert.Equal(1, report.TopUrls[2].Count);
        }

        [Fact]
        public void Analyze_ValidLines_ReturnsTopIpAddresses()
        {
            // Arrange
            var lines = new[]
            {
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100",
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page4/ HTTP/1.1\" 200 100",
                "10.0.0.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "10.0.0.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "10.0.0.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100"
            };

            // Act
            var report = _analyzer.Analyze(lines, _parser, topN: 3);

            // Assert
            Assert.Equal(3, report.TopIpAddresses.Count);
            Assert.Equal("10.0.0.1", report.TopIpAddresses[0].IpAddress);
            Assert.Equal(4, report.TopIpAddresses[0].Count);
            Assert.Equal("10.0.0.2", report.TopIpAddresses[1].IpAddress);
            Assert.Equal(2, report.TopIpAddresses[1].Count);
            Assert.Equal("10.0.0.3", report.TopIpAddresses[2].IpAddress);
            Assert.Equal(1, report.TopIpAddresses[2].Count);
        }

        [Fact]
        public void Analyze_EmptyLines_ReturnsEmptyReport()
        {
            // Arrange
            var lines = Array.Empty<string>();

            // Act
            var report = _analyzer.Analyze(lines, _parser);

            // Assert
            Assert.Equal(0, report.UniqueIpCount);
            Assert.Empty(report.TopUrls);
            Assert.Empty(report.TopIpAddresses);
        }

        [Fact]
        public void Analyze_RequestingMoreThanAvailable_ReturnsAll()
        {
            // Arrange
            var lines = new[]
            {
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100"
            };

            // Act
            var report = _analyzer.Analyze(lines, _parser, topN: 5);

            // Assert
            Assert.Equal(2, report.TopUrls.Count);
            Assert.Equal(2, report.TopIpAddresses.Count);
        }

        [Fact]
        public void Analyze_WithInvalidLines_SkipsInvalidLines()
        {
            // Arrange
            var lines = new[]
            {
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "invalid line",
                "",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100"
            };

            // Act
            var report = _analyzer.Analyze(lines, _parser);

            // Assert
            Assert.Equal(2, report.UniqueIpCount);
        }

        [Fact]
        public void Analyze_TiedCounts_SortsAlphabetically()
        {
            // Arrange - URLs with same count should be sorted alphabetically
            var lines = new[]
            {
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /zebra/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /apple/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /mango/ HTTP/1.1\" 200 100"
            };

            // Act
            var report = _analyzer.Analyze(lines, _parser, topN: 3);

            // Assert - alphabetical order for tied counts
            Assert.Equal("/apple/", report.TopUrls[0].Url);
            Assert.Equal("/mango/", report.TopUrls[1].Url);
            Assert.Equal("/zebra/", report.TopUrls[2].Url);
        }
    }
}
