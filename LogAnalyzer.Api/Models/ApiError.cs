namespace LogAnalyzer.Api.Models
{
    public class ApiError
    {
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public int StatusCode { get; set; }
        public string TraceId { get; set; } = string.Empty;
    }
}
