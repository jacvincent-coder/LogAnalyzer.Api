using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Api.Models
{
    public class LogUploadRequest
    {
        [FromForm(Name = "logFile")]
        public IFormFile LogFile { get; set; }
    }

}
