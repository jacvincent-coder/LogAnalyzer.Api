using LogAnalyzer.Api.Controllers;
using LogAnalyzer.Api.Models;
using LogAnalyzer.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;
using Xunit;

namespace LogAnalyzer.Tests.Controllers
{
    public class LogControllerTests
    {
        private readonly Mock<ILogAnalyzerService> _mockService = new();

        private LogController CreateController()
        {
            return new LogController(_mockService.Object);
        }

        private IFormFile CreateFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "LogFile", "test.log");
        }

        [Fact]
        public async Task Analyze_Should_Return_BadRequest_When_File_Is_Null()
        {
            var controller = CreateController();

            var request = new LogUploadRequest { LogFile = null! };
            var response = await controller.Analyze(request);

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Analyze_Should_Return_BadRequest_When_File_Is_Empty()
        {
            var controller = CreateController();

            var emptyFile = new FormFile(new MemoryStream(), 0, 0, "LogFile", "empty.log");
            var response = await controller.Analyze(new LogUploadRequest { LogFile = emptyFile });

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Analyze_Should_Return_Ok_When_Service_Succeeds()
        {
            var controller = CreateController();

            _mockService.Setup(s => s.AnalyzeLogFileAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(new LogAnalysisResult { UniqueIpCount = 5 });

            var response = await controller.Analyze(new LogUploadRequest { LogFile = CreateFile("test") });

            var ok = Assert.IsType<OkObjectResult>(response.Result);
            var model = Assert.IsType<LogAnalysisResult>(ok.Value);

            Assert.Equal(5, model.UniqueIpCount);
        }

        [Fact]
        public async Task Analyze_Should_Call_Service_Exactly_Once()
        {
            var controller = CreateController();

            _mockService.Setup(s => s.AnalyzeLogFileAsync(It.IsAny<Stream>()))
                        .ReturnsAsync(new LogAnalysisResult());

            await controller.Analyze(new LogUploadRequest { LogFile = CreateFile("abc") });

            _mockService.Verify(s => s.AnalyzeLogFileAsync(It.IsAny<Stream>()), Times.Once);
        }
    }
}
