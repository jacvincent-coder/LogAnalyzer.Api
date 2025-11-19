using LogAnalyzer.Api.Models;

namespace LogAnalyzer.Api.Services
{
    public interface ILogAnalyzerService
    {
        Task<LogAnalysisResult> AnalyzeLogFileAsync(Stream logStream);
    }
}
