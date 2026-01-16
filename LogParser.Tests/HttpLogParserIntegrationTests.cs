using Xunit;

namespace LogParser.Tests
{
    /// <summary>
    /// Integration tests for HttpLogParser facade.
    /// Tests that the facade correctly wires all components together.
    /// </summary>
    public class HttpLogParserTests
    {
        private readonly HttpLogParser _parser;

        public HttpLogParserTests()
        {
            _parser = new HttpLogParser();
        }

        [Fact]
        public void ParseAndGenerateReport_ValidFile_ReturnsFormattedReport()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseAndGenerateReport(tempFile);

                // Assert
                Assert.Contains("HTTP LOG ANALYSIS REPORT", report);
                Assert.Contains("Number of Unique IP Addresses: 2", report);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void ParseLogFileStreaming_ValidFile_ReturnsCorrectReport()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile);

                // Assert
                Assert.Equal(2, report.UniqueIpCount);
                Assert.Equal("/page1/", report.TopUrls[0].Url);
                Assert.Equal(2, report.TopUrls[0].Count);
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
        public void ParseLogFileStreaming_CustomTopN_ReturnsCorrectCount()
        {
            // Arrange
            var tempFile = CreateTempLogFile(
                "192.168.1.1 - - [10/Jul/2018:22:21:28 +0200] \"GET /page1/ HTTP/1.1\" 200 100",
                "192.168.1.2 - - [10/Jul/2018:22:21:28 +0200] \"GET /page2/ HTTP/1.1\" 200 100",
                "192.168.1.3 - - [10/Jul/2018:22:21:28 +0200] \"GET /page3/ HTTP/1.1\" 200 100",
                "192.168.1.4 - - [10/Jul/2018:22:21:28 +0200] \"GET /page4/ HTTP/1.1\" 200 100",
                "192.168.1.5 - - [10/Jul/2018:22:21:28 +0200] \"GET /page5/ HTTP/1.1\" 200 100"
            );

            try
            {
                // Act
                var report = _parser.ParseLogFileStreaming(tempFile, topN: 2);

                // Assert
                Assert.Equal(2, report.TopUrls.Count);
                Assert.Equal(2, report.TopIpAddresses.Count);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

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
