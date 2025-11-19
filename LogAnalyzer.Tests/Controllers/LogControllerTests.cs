using LogAnalyzer.Api.Controllers;
using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace LogAnalyzer.Tests.Controllers
{
    public class LogControllerTests
    {
        private LogController CreateController(Mock<ILogAnalyzerService> mockService)
        {
            var logger = new Mock<ILogger<LogController>>();
            return new LogController(mockService.Object, logger.Object);
        }

        private IFormFile CreateFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "LogFile", "test.log");
        }

        [Fact]
        public async Task Analyze_Should_Return_BadRequest_When_File_Is_Null()
        {
            var mockService = new Mock<ILogAnalyzerService>();
            var controller = CreateController(mockService);

            var request = new LogUploadRequest { LogFile = null! };

            var result = await controller.Analyze(request);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Analyze_Should_Return_BadRequest_When_File_Is_Empty()
        {
            var mockService = new Mock<ILogAnalyzerService>();
            var controller = CreateController(mockService);

            var emptyFile = new FormFile(new MemoryStream(), 0, 0, "LogFile", "empty.log");
            var result = await controller.Analyze(new LogUploadRequest { LogFile = emptyFile });

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Analyze_Should_Return_Ok_When_Service_Succeeds()
        {
            var mockService = new Mock<ILogAnalyzerService>();

            mockService.Setup(s => s.AnalyzeLogFileAsync(It.IsAny<Stream>()))
                       .ReturnsAsync(new LogAnalysisResult { UniqueIpCount = 5 });

            var controller = CreateController(mockService);

            var result = await controller.Analyze(new LogUploadRequest { LogFile = CreateFile("data") });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<LogAnalysisResult>(ok.Value);

            Assert.Equal(5, value.UniqueIpCount);
        }

        [Fact]
        public async Task Analyze_Should_Call_Service_Exactly_Once()
        {
            var mockService = new Mock<ILogAnalyzerService>();

            mockService.Setup(s => s.AnalyzeLogFileAsync(It.IsAny<Stream>()))
                       .ReturnsAsync(new LogAnalysisResult());

            var controller = CreateController(mockService);

            await controller.Analyze(new LogUploadRequest { LogFile = CreateFile("abc") });

            mockService.Verify(s => s.AnalyzeLogFileAsync(It.IsAny<Stream>()), Times.Once);
        }
    }
}
