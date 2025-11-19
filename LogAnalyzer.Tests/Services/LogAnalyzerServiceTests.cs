using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Services;
using LogAnalyzer.Api.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace LogAnalyzer.Tests.Services
{
    public class LogAnalyzerServiceTests
    {
        private MemoryStream CreateStream(string content) =>
            new MemoryStream(Encoding.UTF8.GetBytes(content));

        private delegate void TryParseCallback(string line, out LogEntry? entry);

        [Fact]
        public async Task Should_Count_Unique_Ips()
        {
            var entries = new Queue<LogEntry>(new[]
            {
                new LogEntry { IpAddress = "1.1.1.1", Url = "/a" },
                new LogEntry { IpAddress = "2.2.2.2", Url = "/b" },
                new LogEntry { IpAddress = "1.1.1.1", Url = "/c" }
            });

            var parser = new Mock<ILogParser>();
            parser.Setup(p => p.TryParse(It.IsAny<string>(), out It.Ref<LogEntry?>.IsAny))
                  .Callback(new TryParseCallback((string _, out LogEntry? e) => e = entries.Dequeue()))
                  .Returns(true);

            var logger = new Mock<ILogger<LogAnalyzerService>>();
            var service = new LogAnalyzerService(parser.Object, logger.Object);

            var result = await service.AnalyzeLogFileAsync(CreateStream("x\ny\nz"));

            Assert.Equal(2, result.UniqueIpCount);
        }

        [Fact]
        public async Task Should_Ignore_Malformed_Lines()
        {
            var parser = new Mock<ILogParser>();

            parser.Setup(p => p.TryParse("good", out It.Ref<LogEntry?>.IsAny))
                  .Callback(new TryParseCallback((string _, out LogEntry? e) =>
                  {
                      e = new LogEntry { IpAddress = "1.1.1.1", Url = "/ok" };
                  }))
                  .Returns(true);

            parser.Setup(p => p.TryParse("bad", out It.Ref<LogEntry?>.IsAny))
                  .Returns(false);

            var logger = new Mock<ILogger<LogAnalyzerService>>();
            var service = new LogAnalyzerService(parser.Object, logger.Object);

            var result = await service.AnalyzeLogFileAsync(CreateStream("good\nbad"));

            Assert.Single(result.TopUrls);
            Assert.Equal("/ok", result.TopUrls.First());
        }

        [Fact]
        public async Task Should_Handle_Empty_Stream()
        {
            var parser = new Mock<ILogParser>();
            var logger = new Mock<ILogger<LogAnalyzerService>>();

            var service = new LogAnalyzerService(parser.Object, logger.Object);

            var result = await service.AnalyzeLogFileAsync(new MemoryStream());

            Assert.Equal(0, result.UniqueIpCount);
            Assert.Empty(result.TopIpAddresses);
            Assert.Empty(result.TopUrls);
        }

        [Fact]
        public async Task Should_Calculate_Top_Urls()
        {
            var entries = new Queue<LogEntry>(new[]
            {
                new LogEntry { IpAddress = "1", Url = "/a" },
                new LogEntry { IpAddress = "1", Url = "/b" },
                new LogEntry { IpAddress = "1", Url = "/a" },
            });

            var parser = new Mock<ILogParser>();
            parser.Setup(p => p.TryParse(It.IsAny<string>(), out It.Ref<LogEntry?>.IsAny))
                  .Callback(new TryParseCallback((string _, out LogEntry? e) => e = entries.Dequeue()))
                  .Returns(true);

            var logger = new Mock<ILogger<LogAnalyzerService>>();
            var service = new LogAnalyzerService(parser.Object, logger.Object);

            var result = await service.AnalyzeLogFileAsync(CreateStream("x\ny\nz"));

            Assert.Equal("/a", result.TopUrls.First());
        }
    }
}
