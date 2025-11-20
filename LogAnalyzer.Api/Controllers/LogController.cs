using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class LogController : ControllerBase
    {
        private readonly ILogAnalyzerService _logAnalyzerService;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogAnalyzerService logAnalyzerService,
                             ILogger<LogController> logger)
        {
            _logAnalyzerService = logAnalyzerService;
            _logger = logger;
        }

        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<LogAnalysisResult>> Analyze([FromForm] LogUploadRequest request)
        {
            if (request.LogFile == null || request.LogFile.Length == 0)
            {
                _logger.LogWarning("Empty file submitted.");
                return BadRequest("Please upload a valid log file.");
            }

            _logger.LogInformation("Received log file: {fileName}", request.LogFile.FileName);

            using var stream = request.LogFile.OpenReadStream();
            var result = await _logAnalyzerService.AnalyzeLogFileAsync(stream);

            _logger.LogInformation("Returning analysis result.");

            return Ok(result);
        }
    }
}
