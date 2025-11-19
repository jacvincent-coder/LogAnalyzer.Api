using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogAnalyzerService _logAnalyzerService;

        public LogController(ILogAnalyzerService logAnalyzerService)
        {
            _logAnalyzerService = logAnalyzerService;
        }

        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<LogAnalysisResult>> Analyze([FromForm] LogUploadRequest request)
        {
            if (request.LogFile == null || request.LogFile.Length == 0)
                return BadRequest("Please upload a valid log file.");

            using var stream = request.LogFile.OpenReadStream();
            var result = await _logAnalyzerService.AnalyzeLogFileAsync(stream);

            return Ok(result);
        }

    }
}
