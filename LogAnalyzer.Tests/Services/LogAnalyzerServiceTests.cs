using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Services;
using LogAnalyzer.Api.Utils;
using Moq;
using System.Text;
using Xunit;

namespace LogAnalyzer.Tests.Services
{
    public class LogAnalyzerServiceTests
    {
        private MemoryStream Stream(string s) =>
            new MemoryStream(Encoding.UTF8.GetBytes(s));

        private delegate void TryParseDelegate(string line, out LogEntry? entry);

        [Fact]
        public async Task Should_Count_Unique_Ips()
        {
            var entries = new Queue<LogEntry>(new[]
            {
                new LogEntry { IpAddress = "1.1.1.1", Url = "/a" },
                new LogEntry { IpAddress = "2.2.2.2", Url = "/b" },
                new LogEntry { IpAddress = "1.1.1.1", Url = "/c" },
            });

            var parser = new Mock<ILogParser>();
            parser.Setup(p => p.TryParse(It.IsAny<string>(), out It.Ref<LogEntry?>.IsAny))
                .Callback(new TryParseDelegate((string _, out LogEntry? outEntry) =>
                {
                    outEntry = entries.Dequeue();
                }))
                .Returns(true);

            var service = new LogAnalyzerService(parser.Object);

            var result = await service.AnalyzeLogFileAsync(Stream("x\ny\nz"));

            Assert.Equal(2, result.UniqueIpCount);
        }

        [Fact]
        public async Task Should_Calculate_Top_Three_Urls()
        {
            var entries = new Queue<LogEntry>(new[]
            {
                new LogEntry { IpAddress = "1", Url = "/a" },
                new LogEntry { IpAddress = "1", Url = "/b" },
                new LogEntry { IpAddress = "1", Url = "/a" },
            });

            var parser = new Mock<ILogParser>();
            parser.Setup(p => p.TryParse(It.IsAny<string>(), out It.Ref<LogEntry?>.IsAny))
                .Callback(new TryParseDelegate((string _, out LogEntry? outEntry) =>
                {
                    outEntry = entries.Dequeue();
                }))
                .Returns(true);

            var service = new LogAnalyzerService(parser.Object);

            var result = await service.AnalyzeLogFileAsync(Stream("x\ny\nz"));

            Assert.Equal("/a", result.TopUrls.First());
            Assert.Equal(2, result.TopUrls.Count()); // "/a" and "/b"
        }

        [Fact]
        public async Task Should_Ignore_Malformed_Lines()
        {
            var parser = new Mock<ILogParser>();

            // Good line "good"
            parser.Setup(p => p.TryParse("good", out It.Ref<LogEntry?>.IsAny))
                .Callback(new TryParseDelegate((string _, out LogEntry? e) =>
                {
                    e = new LogEntry { IpAddress = "1.1.1.1", Url = "/ok" };
                }))
                .Returns(true);

            // Bad line "bad"
            parser.Setup(p => p.TryParse("bad", out It.Ref<LogEntry?>.IsAny))
                .Returns(false);

            var service = new LogAnalyzerService(parser.Object);

            var result = await service.AnalyzeLogFileAsync(Stream("good\nbad"));

            Assert.Single(result.TopUrls);
            Assert.Equal("/ok", result.TopUrls.First());
        }


        [Fact]
        public async Task Should_Handle_Empty_Stream()
        {
            var parser = new Mock<ILogParser>();
            var service = new LogAnalyzerService(parser.Object);

            var result = await service.AnalyzeLogFileAsync(new MemoryStream());

            Assert.Empty(result.TopUrls);
            Assert.Empty(result.TopIpAddresses);
            Assert.Equal(0, result.UniqueIpCount);
        }

        [Fact]
        public async Task Should_Tally_Ips_Correctly()
        {
            var entries = new Queue<LogEntry>(new[]
            {
                new LogEntry { IpAddress = "9.9.9.9", Url = "/x" },
                new LogEntry { IpAddress = "9.9.9.9", Url = "/y" },
                new LogEntry { IpAddress = "8.8.8.8", Url = "/z" },
            });

            var parser = new Mock<ILogParser>();
            parser.Setup(p => p.TryParse(It.IsAny<string>(), out It.Ref<LogEntry?>.IsAny))
                .Callback(new TryParseDelegate((string _, out LogEntry? outEntry) =>
                {
                    outEntry = entries.Dequeue();
                }))
                .Returns(true);

            var service = new LogAnalyzerService(parser.Object);

            var result = await service.AnalyzeLogFileAsync(Stream("a\nb\nc"));

            Assert.Equal("9.9.9.9", result.TopIpAddresses.First());
        }

        [Fact]
        public async Task Should_Process_Large_Log()
        {
            var entries = new Queue<LogEntry>(
                Enumerable.Range(0, 1000)
                .Select(i => new LogEntry
                {
                    IpAddress = $"1.1.1.{i % 5}",
                    Url = $"/url{i % 10}"
                })
            );

            var parser = new Mock<ILogParser>();
            parser.Setup(p => p.TryParse(It.IsAny<string>(), out It.Ref<LogEntry?>.IsAny))
                .Callback(new TryParseDelegate((string _, out LogEntry? outEntry) =>
                {
                    outEntry = entries.Dequeue();
                }))
                .Returns(true);

            var service = new LogAnalyzerService(parser.Object);

            var fakeLogContent = string.Join("\n", Enumerable.Range(0, 1000));
            var result = await service.AnalyzeLogFileAsync(Stream(fakeLogContent));

            Assert.Equal(5, result.UniqueIpCount);
            Assert.True(result.TopUrls.Count() <= 3);
        }
    }
}
