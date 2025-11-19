using LogAnalyzer.Api.Models;

namespace LogAnalyzer.Api.Utils
{
    public interface ILogParser
    {
        bool TryParse(string line, out LogEntry? entry);
    }
}
