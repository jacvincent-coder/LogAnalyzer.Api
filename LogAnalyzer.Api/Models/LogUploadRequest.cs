using Microsoft.AspNetCore.Http;

namespace LogAnalyzer.Api.Models
{
    public class LogUploadRequest
    {
        public IFormFile LogFile { get; set; }
    }
}
