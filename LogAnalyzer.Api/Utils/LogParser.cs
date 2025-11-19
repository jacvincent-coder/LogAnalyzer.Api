using System.Text.RegularExpressions;
using LogAnalyzer.Api.Models;

namespace LogAnalyzer.Api.Utils
{
    public interface ILogParser
    {
        bool TryParse(string line, out LogEntry? entry);
    }

    public class LogParser : ILogParser
    {
        // Capture IP and request target from log line
        private static readonly Regex LogRegex = new(
            @"^(?<ip>\d{1,3}(?:\.\d{1,3}){3}).*?""(?<method>[A-Z]+)\s+(?<url>\S+)",
            RegexOptions.Compiled);

        public bool TryParse(string line, out LogEntry? entry)
        {
            entry = null;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            var match = LogRegex.Match(line);
            if (!match.Success)
                return false;

            var ip = match.Groups["ip"].Value;
            var method = match.Groups["method"].Value;
            var url = NormalizeUrl(match.Groups["url"].Value);

            entry = new LogEntry
            {
                IpAddress = ip,
                Url = url
            };

            return true;
        }

        private static string NormalizeUrl(string rawUrl)
        {
            if (rawUrl.StartsWith("http://") || rawUrl.StartsWith("https://"))
            {
                if (Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri))
                {
                    return uri.PathAndQuery;
                }
            }

            return rawUrl;
        }
    }
}
