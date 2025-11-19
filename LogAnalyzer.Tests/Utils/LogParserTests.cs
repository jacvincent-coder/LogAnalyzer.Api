using LogAnalyzer.Api.Utils;
using LogAnalyzer.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LogAnalyzer.Tests.Utils
{
    public class LogParserTests
    {
        private readonly Mock<ILogger<LogParser>> _mockLogger = new();

        [Fact]
        public void Should_Parse_Valid_Line()
        {
            var parser = new LogParser(_mockLogger.Object);
            var line = @"177.71.128.21 - - ""GET /home HTTP/1.1""";

            var success = parser.TryParse(line, out var entry);

            Assert.True(success);
            Assert.Equal("177.71.128.21", entry!.IpAddress);
            Assert.Equal("/home", entry.Url);
        }

        [Fact]
        public void Should_Normalize_Absolute_Url()
        {
            var parser = new LogParser(_mockLogger.Object);
            var line = @"1.1.1.1 - - ""GET http://example.com/faq/ HTTP/1.1""";

            var success = parser.TryParse(line, out var entry);

            Assert.True(success);
            Assert.Equal("/faq/", entry!.Url);
        }

        [Fact]
        public void Should_Return_False_For_Malformed_Line()
        {
            var parser = new LogParser(_mockLogger.Object);
            var success = parser.TryParse("junk", out _);

            Assert.False(success);
        }
    }
}
