using LogAnalyzer.Api.Utils;
using Xunit;

namespace LogAnalyzer.Tests.Utils
{
    public class LogParserTests
    {
        private readonly LogParser _parser = new();

        [Fact]
        public void Should_Parse_Valid_Line()
        {
            var line = @"177.71.128.21 - - ""GET /home HTTP/1.1""";

            var ok = _parser.TryParse(line, out var entry);

            Assert.True(ok);
            Assert.Equal("177.71.128.21", entry!.IpAddress);
            Assert.Equal("/home", entry.Url);
        }

        [Fact]
        public void Should_Normalize_Absolute_Url()
        {
            var line = @"1.1.1.1 - - ""GET http://example.com/faq/ HTTP/1.1""";

            var ok = _parser.TryParse(line, out var entry);

            Assert.True(ok);
            Assert.Equal("/faq/", entry!.Url);
        }

        [Fact]
        public void Should_Return_False_For_Malformed_Line()
        {
            var ok = _parser.TryParse("junk", out _);
            Assert.False(ok);
        }

        [Fact]
        public void Should_Handle_Post_Put_Methods()
        {
            var line = @"9.9.9.9 - - ""POST /submit HTTP/1.1""";

            var ok = _parser.TryParse(line, out var entry);

            Assert.True(ok);
            Assert.Equal("/submit", entry!.Url);
        }
    }
}
