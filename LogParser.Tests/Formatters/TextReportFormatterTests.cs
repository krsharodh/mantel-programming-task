using LogParser.Formatters;
using LogParser.Models;
using Xunit;

namespace LogParser.Tests.Formatters
{
    public class TextReportFormatterTests
    {
        private readonly TextReportFormatter _formatter;

        public TextReportFormatterTests()
        {
            _formatter = new TextReportFormatter();
        }

        [Fact]
        public void Format_ValidReport_ContainsUniqueIpCount()
        {
            // Arrange
            var report = new LogReport
            {
                UniqueIpCount = 5,
                TopUrls = new List<(string Url, int Count)>(),
                TopIpAddresses = new List<(string IpAddress, int Count)>()
            };

            // Act
            var result = _formatter.Format(report);

            // Assert
            Assert.Contains("Number of Unique IP Addresses: 5", result);
        }

        [Fact]
        public void Format_ValidReport_ContainsTopUrls()
        {
            // Arrange
            var report = new LogReport
            {
                UniqueIpCount = 1,
                TopUrls = new List<(string Url, int Count)>
                {
                    ("/page1/", 10),
                    ("/page2/", 5)
                },
                TopIpAddresses = new List<(string IpAddress, int Count)>()
            };

            // Act
            var result = _formatter.Format(report);

            // Assert
            Assert.Contains("/page1/ (10 visits)", result);
            Assert.Contains("/page2/ (5 visits)", result);
        }

        [Fact]
        public void Format_ValidReport_ContainsTopIpAddresses()
        {
            // Arrange
            var report = new LogReport
            {
                UniqueIpCount = 2,
                TopUrls = new List<(string Url, int Count)>(),
                TopIpAddresses = new List<(string IpAddress, int Count)>
                {
                    ("192.168.1.1", 100),
                    ("10.0.0.1", 50)
                }
            };

            // Act
            var result = _formatter.Format(report);

            // Assert
            Assert.Contains("192.168.1.1 (100 requests)", result);
            Assert.Contains("10.0.0.1 (50 requests)", result);
        }

        [Fact]
        public void Format_ValidReport_ContainsHeader()
        {
            // Arrange
            var report = new LogReport
            {
                UniqueIpCount = 0,
                TopUrls = new List<(string Url, int Count)>(),
                TopIpAddresses = new List<(string IpAddress, int Count)>()
            };

            // Act
            var result = _formatter.Format(report);

            // Assert
            Assert.Contains("HTTP LOG ANALYSIS REPORT", result);
        }

        [Fact]
        public void Format_EmptyReport_ReturnsValidFormat()
        {
            // Arrange
            var report = new LogReport
            {
                UniqueIpCount = 0,
                TopUrls = new List<(string Url, int Count)>(),
                TopIpAddresses = new List<(string IpAddress, int Count)>()
            };

            // Act
            var result = _formatter.Format(report);

            // Assert
            Assert.NotEmpty(result);
            Assert.Contains("Number of Unique IP Addresses: 0", result);
        }
    }
}
