using Xunit;

namespace LogParser.Tests
{
    public class HttpLogParserTests
    {
        private readonly HttpLogParser _parser;

        public HttpLogParserTests()
        {
            _parser = new HttpLogParser();
        }

        #region ParseLogLine Tests

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

        [Fact]
        public void ParseLogLine_ValidTimestamp_ParsesCorrectly()
        {
            // Arrange
            string line = "177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] \"GET /test/ HTTP/1.1\" 200 3574";

            // Act
            var entry = _parser.ParseLogLine(line);

            // Assert
            Assert.NotNull(entry);
            // Verify the timestamp is parsed (not default) - exact value depends on local timezone
            Assert.NotEqual(DateTime.MinValue, entry.Timestamp);
            Assert.Equal(2018, entry.Timestamp.Year);
            Assert.Equal(7, entry.Timestamp.Month);
            Assert.Equal(21, entry.Timestamp.Minute);
            Assert.Equal(28, entry.Timestamp.Second);
        }

        #endregion

        #region ParseLogFileStreaming Tests

        [Fact]
        public void ParseLogFileStreaming_ValidFile_ReturnsCorrectUniqueIpCount()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100",
                "192.168.1.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page4/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile);

                // Assert
                Assert.Equal(3, report.UniqueIpCount);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseLogFileStreaming_ValidFile_ReturnsTopUrls()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile, topN: 3);

                // Assert
                Assert.Equal(3, report.TopUrls.Count);
                Assert.Equal("/page1/", report.TopUrls[0].Item1);
                Assert.Equal(3, report.TopUrls[0].Item2);
                Assert.Equal("/page2/", report.TopUrls[1].Item1);
                Assert.Equal(2, report.TopUrls[1].Item2);
                Assert.Equal("/page3/", report.TopUrls[2].Item1);
                Assert.Equal(1, report.TopUrls[2].Item2);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseLogFileStreaming_ValidFile_ReturnsTopIpAddresses()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100",
                "10.0.0.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page4/ HTTP/1.1\" 200 100",
                "10.0.0.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "10.0.0.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "10.0.0.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile, topN: 3);

                // Assert
                Assert.Equal(3, report.TopIpAddresses.Count);
                Assert.Equal("10.0.0.1", report.TopIpAddresses[0].Item1);
                Assert.Equal(4, report.TopIpAddresses[0].Item2);
                Assert.Equal("10.0.0.2", report.TopIpAddresses[1].Item1);
                Assert.Equal(2, report.TopIpAddresses[1].Item2);
                Assert.Equal("10.0.0.3", report.TopIpAddresses[2].Item1);
                Assert.Equal(1, report.TopIpAddresses[2].Item2);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseLogFileStreaming_EmptyFile_ReturnsEmptyReport()
        {
            // Arrange
            var tempFile = CreateTempLogFile();

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile);

                // Assert
                Assert.Equal(0, report.UniqueIpCount);
                Assert.Empty(report.TopUrls);
                Assert.Empty(report.TopIpAddresses);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseLogFileStreaming_RequestingMoreThanAvailable_ReturnsAll()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile, topN: 5);

                // Assert
                Assert.Equal(2, report.TopUrls.Count);
                Assert.Equal(2, report.TopIpAddresses.Count);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        #endregion

        #region Helper Methods

        private static string CreateTempLogFile(params string[] lines)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, lines);
            return tempFile;
        }

        #endregion
    }
}
