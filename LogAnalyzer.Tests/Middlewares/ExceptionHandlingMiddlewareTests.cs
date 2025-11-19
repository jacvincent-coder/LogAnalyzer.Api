using LogAnalyzer.Api.Middleware;
using LogAnalyzer.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace LogAnalyzer.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Should_Return_500_And_Log_Error_When_Exception_Occurs()
        {
            var logger = new Mock<ILogger<ExceptionHandlingMiddleware>>();

            RequestDelegate next = (ctx) => throw new Exception("boom");
            var middleware = new ExceptionHandlingMiddleware(next, logger.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            Assert.Equal(500, context.Response.StatusCode);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var error = await JsonSerializer.DeserializeAsync<ApiError>(context.Response.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            Assert.Equal("An unexpected error occurred.", error!.Message);
            Assert.Equal(500, error.StatusCode);

            logger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
