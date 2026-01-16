using LogParser.Models;
using LogParser.Parsers;
using Xunit;

namespace LogParser.Tests.Parsers
{
    public class CommonLogFormatParserTests
    {
        private readonly CommonLogFormatParser _parser;

        public CommonLogFormatParserTests()
        {
            _parser = new CommonLogFormatParser();
        }

        [Fact]
        public void Parse_ValidLine_ReturnsLogEntry()
        {
            // Arrange
            string line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /intranet-analytics/ HTTP/1.1\" 200 3574";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal("177.71.128.21", entry.IpAddress);
            Assert.Equal("GET", entry.HttpMethod);
            Assert.Equal("/intranet-analytics/", entry.Url);
            Assert.Equal(200, entry.StatusCode);
            Assert.Equal(3574, entry.ResponseSize);
        }

        [Fact]
        public void Parse_EmptyLine_ReturnsNull()
        {
            // Arrange
            string line = "";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.Null(entry);
        }

        [Fact]
        public void Parse_InvalidFormat_ReturnsNull()
        {
            // Arrange
            string line = "invalid log line";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.Null(entry);
        }

        [Fact]
        public void Parse_PostRequest_ParsesCorrectly()
        {
            // Arrange
            string line = "192.168.1.100 - admin [10/Jul/2018:22:21:28 +0200] \"POST /api/submit HTTP/1.1\" 201 1234";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal("192.168.1.100", entry.IpAddress);
            Assert.Equal("POST", entry.HttpMethod);
            Assert.Equal("/api/submit", entry.Url);
            Assert.Equal(201, entry.StatusCode);
        }

        [Fact]
        public void Parse_404Response_ParsesCorrectly()
        {
            // Arrange
            string line = "177.71.128.21 - - [10/Jul/2018:22:23:08 +0200] \"GET /this/page/does/not/exist/ HTTP/1.1\" 404 3574";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal(404, entry.StatusCode);
        }

        [Fact]
        public void Parse_ValidTimestamp_ParsesCorrectly()
        {
            // Arrange
            string line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /test/ HTTP/1.1\" 200 3574";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.NotNull(entry);
            Assert.NotEqual(DateTime.MinValue, entry.Timestamp);
            Assert.Equal(2018, entry.Timestamp.Year);
            Assert.Equal(7, entry.Timestamp.Month);
            Assert.Equal(21, entry.Timestamp.Minute);
            Assert.Equal(28, entry.Timestamp.Second);
        }

        [Fact]
        public void Parse_NullLine_ReturnsNull()
        {
            // Act
            var entry = _parser.Parse(null!);

            // Assert
            Assert.Null(entry);
        }

        [Fact]
        public void Parse_WhitespaceLine_ReturnsNull()
        {
            // Arrange
            string line = "   ";

            // Act
            var entry = _parser.Parse(line);

            // Assert
            Assert.Null(entry);
        }
    }
}
