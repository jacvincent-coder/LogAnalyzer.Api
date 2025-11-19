using Microsoft.Extensions.Logging;
using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Utils;

namespace LogAnalyzer.Api.Services
{
    public class LogAnalyzerService : ILogAnalyzerService
    {
        private readonly ILogParser _parser;
        private readonly ILogger<LogAnalyzerService> _logger;

        public LogAnalyzerService(ILogParser parser, ILogger<LogAnalyzerService> logger)
        {
            _parser = parser;
            _logger = logger;
        }

        public async Task<LogAnalysisResult> AnalyzeLogFileAsync(Stream logStream)
        {
            _logger.LogInformation("Starting log file analysis...");

            var ipCounts = new Dictionary<string, int>();
            var urlCounts = new Dictionary<string, int>();
            var uniqueIps = new HashSet<string>();

            using var reader = new StreamReader(logStream);
            int lineNumber = 0;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                lineNumber++;

                if (line == null) continue;

                if (!_parser.TryParse(line, out var entry))
                {
                    _logger.LogWarning("Skipping malformed line {lineNumber}: {line}", lineNumber, line);
                    continue;
                }

                _logger.LogDebug("Parsed log entry. IP={ip}, URL={url}", entry!.IpAddress, entry.Url);

                uniqueIps.Add(entry.IpAddress);
                ipCounts[entry.IpAddress] = ipCounts.GetValueOrDefault(entry.IpAddress, 0) + 1;
                urlCounts[entry.Url] = urlCounts.GetValueOrDefault(entry.Url, 0) + 1;
            }

            _logger.LogInformation("Finished parsing log. Unique IPs: {count}", uniqueIps.Count);

            return new LogAnalysisResult
            {
                UniqueIpCount = uniqueIps.Count,
                TopUrls = urlCounts.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key),
                TopIpAddresses = ipCounts.OrderByDescending(x => x.Value).Take(3).Select(x => x.Key)
            };
        }
    }
}
