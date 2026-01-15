using Xunit;
using LogParser;

namespace LogParser.Tests
{
    public class HttpLogParserTests
    {
        private readonly HttpLogParser _parser;

        public HttpLogParserTests()
        {
            _parser = new HttpLogParser();
        }

        [Fact]
        public void ParseLogLine_ValidLine_ReturnsLogEntry()
        {
            // Arrange
            string line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /intranet-analytics/ HTTP/1.1\" 200 3574";

            // Act
            var entry = _parser.ParseLogLine(line);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal("177.71.128.21", entry.IpAddress);
            Assert.Equal("GET", entry.HttpMethod);
            Assert.Equal("/intranet-analytics/", entry.Url);
            Assert.Equal(200, entry.StatusCode);
            Assert.Equal(3574, entry.ResponseSize);
        }

        [Fact]
        public void ParseLogLine_EmptyLine_ReturnsNull()
        {
            // Arrange
            string line = "";

            // Act
            var entry = _parser.ParseLogLine(line);

            // Assert
            Assert.Null(entry);
        }

        [Fact]
        public void ParseLogLine_InvalidFormat_ReturnsNull()
        {
            // Arrange
            string line = "invalid log line";

            // Act
            var entry = _parser.ParseLogLine(line);

            // Assert
            Assert.Null(entry);
        }

        [Fact]
        public void GetUniqueIpAddressCount_MultipleEntries_ReturnsCorrectCount()
        {
            // Arrange
            var entries = new List<LogEntry>
            {
                new LogEntry { IpAddress = "192.168.1.1" },
                new LogEntry { IpAddress = "192.168.1.2" },
                new LogEntry { IpAddress = "192.168.1.1" },
                new LogEntry { IpAddress = "192.168.1.3" }
            };

            // Act
            var count = _parser.GetUniqueIpAddressCount(entries);

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public void GetUniqueIpAddressCount_EmptyList_ReturnsZero()
        {
            // Arrange
            var entries = new List<LogEntry>();

            // Act
            var count = _parser.GetUniqueIpAddressCount(entries);

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetTopVisitedUrls_ReturnsTopNUrls()
        {
            // Arrange
            var entries = new List<LogEntry>
            {
                new LogEntry { Url = "/page1/" },
                new LogEntry { Url = "/page1/" },
                new LogEntry { Url = "/page1/" },
                new LogEntry { Url = "/page2/" },
                new LogEntry { Url = "/page2/" },
                new LogEntry { Url = "/page3/" }
            };

            // Act
            var topUrls = _parser.GetTopVisitedUrls(entries, 3);

            // Assert
            Assert.Equal(3, topUrls.Count);
            Assert.Equal("/page1/", topUrls[0].Url);
            Assert.Equal(3, topUrls[0].Count);
            Assert.Equal("/page2/", topUrls[1].Url);
            Assert.Equal(2, topUrls[1].Count);
            Assert.Equal("/page3/", topUrls[2].Url);
            Assert.Equal(1, topUrls[2].Count);
        }

        [Fact]
        public void GetTopActiveIpAddresses_ReturnsTopNIps()
        {
            // Arrange
            var entries = new List<LogEntry>
            {
                new LogEntry { IpAddress = "10.0.0.1" },
                new LogEntry { IpAddress = "10.0.0.1" },
                new LogEntry { IpAddress = "10.0.0.1" },
                new LogEntry { IpAddress = "10.0.0.1" },
                new LogEntry { IpAddress = "10.0.0.2" },
                new LogEntry { IpAddress = "10.0.0.2" },
                new LogEntry { IpAddress = "10.0.0.3" }
            };

            // Act
            var topIps = _parser.GetTopActiveIpAddresses(entries, 3);

            // Assert
            Assert.Equal(3, topIps.Count);
            Assert.Equal("10.0.0.1", topIps[0].IpAddress);
            Assert.Equal(4, topIps[0].Count);
            Assert.Equal("10.0.0.2", topIps[1].IpAddress);
            Assert.Equal(2, topIps[1].Count);
            Assert.Equal("10.0.0.3", topIps[2].IpAddress);
            Assert.Equal(1, topIps[2].Count);
        }

        [Fact]
        public void GetTopVisitedUrls_RequestingMoreThanAvailable_ReturnsAll()
        {
            // Arrange
            var entries = new List<LogEntry>
            {
                new LogEntry { Url = "/page1/" },
                new LogEntry { Url = "/page2/" }
            };

            // Act
            var topUrls = _parser.GetTopVisitedUrls(entries, 5);

            // Assert
            Assert.Equal(2, topUrls.Count);
        }

        [Fact]
        public void ParseLogLine_PostRequest_ParsesCorrectly()
        {
            // Arrange
            string line = "192.168.1.100 - admin [10/Jul/2018:22:21:28 +0200] \"POST /api/submit HTTP/1.1\" 201 1234";

            // Act
            var entry = _parser.ParseLogLine(line);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal("192.168.1.100", entry.IpAddress);
            Assert.Equal("POST", entry.HttpMethod);
            Assert.Equal("/api/submit", entry.Url);
            Assert.Equal(201, entry.StatusCode);
        }

        [Fact]
        public void ParseLogLine_404Response_ParsesCorrectly()
        {
            // Arrange
            string line = "177.71.128.21 - - [10/Jul/2018:22:23:08 +0200] \"GET /this/page/does/not/exist/ HTTP/1.1\" 404 3574";

            // Act
            var entry = _parser.ParseLogLine(line);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal(404, entry.StatusCode);
        }
    }
}
