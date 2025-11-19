using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Utils;

namespace LogAnalyzer.Api.Services
{
    public class LogAnalyzerService : ILogAnalyzerService
    {
        private readonly ILogParser _parser;

        public LogAnalyzerService(ILogParser parser)
        {
            _parser = parser;
        }

        public async Task<LogAnalysisResult> AnalyzeLogFileAsync(Stream logStream)
        {
            var ipCounts = new Dictionary<string, int>();
            var urlCounts = new Dictionary<string, int>();
            var uniqueIps = new HashSet<string>();

            using var reader = new StreamReader(logStream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) continue;

                if (!_parser.TryParse(line, out var entry))
                    continue;

                uniqueIps.Add(entry!.IpAddress);

                ipCounts[entry.IpAddress] = ipCounts.GetValueOrDefault(entry.IpAddress, 0) + 1;
                urlCounts[entry.Url] = urlCounts.GetValueOrDefault(entry.Url, 0) + 1;
            }

            return new LogAnalysisResult
            {
                UniqueIpCount = uniqueIps.Count,
                TopUrls = urlCounts.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key),
                TopIpAddresses = ipCounts.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key)
            };
        }
    }
}
