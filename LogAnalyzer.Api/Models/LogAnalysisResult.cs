namespace LogAnalyzer.Api.Models
{
    public class LogAnalysisResult
    {
        public int UniqueIpCount { get; set; }
        public IEnumerable<string> TopUrls { get; set; } = [];
        public IEnumerable<string> TopIpAddresses { get; set; } = [];
    }
}
