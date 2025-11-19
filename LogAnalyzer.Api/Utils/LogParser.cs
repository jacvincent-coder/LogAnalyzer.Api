using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using LogAnalyzer.Api.Models;

namespace LogAnalyzer.Api.Utils
{
    public class LogParser : ILogParser
    {
        private readonly ILogger<LogParser> _logger;

        public LogParser(ILogger<LogParser> logger)
        {
            _logger = logger;
        }

        private static readonly Regex LogRegex = new(
            @"^(?<ip>\d{1,3}(?:\.\d{1,3}){3}).*?""(?<method>[A-Z]+)\s+(?<url>\S+)",
            RegexOptions.Compiled);

        public bool TryParse(string line, out LogEntry? entry)
        {
            entry = null;

            if (string.IsNullOrWhiteSpace(line))
            {
                _logger.LogWarning("Encountered blank or whitespace-only log line.");
                return false;
            }

            var match = LogRegex.Match(line);
            if (!match.Success)
            {
                _logger.LogWarning("Failed to parse log line: {line}", line);
                return false;
            }

            entry = new LogEntry
            {
                IpAddress = match.Groups["ip"].Value,
                Url = NormalizeUrl(match.Groups["url"].Value)
            };

            return true;
        }

        private static string NormalizeUrl(string rawUrl)
        {
            if (rawUrl.StartsWith("http://") || rawUrl.StartsWith("https://"))
            {
                if (Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri))
                    return uri.PathAndQuery;
            }

            return rawUrl;
        }
    }
}
