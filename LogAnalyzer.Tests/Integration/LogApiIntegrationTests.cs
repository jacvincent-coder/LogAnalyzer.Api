using LogAnalyzer.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace LogAnalyzer.Tests.Integration
{
    public class LogApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LogApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Analyze_Should_Return_200_For_Valid_File()
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("1.1.1.1 - - \"GET /a HTTP/1.1\""), "LogFile", "log.txt");

            var response = await _client.PostAsync("/api/log/analyze", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Analyze_Should_Return_400_For_Empty_File()
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(Array.Empty<byte>()), "LogFile", "empty.txt");

            var response = await _client.PostAsync("/api/log/analyze", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Analyze_Should_Trigger_Middleware_For_Exception()
        {
            // Use a huge file to force possible errors OR test unknown behavior
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("INVALID LOG ENTRY"), "LogFile", "bad.log");

            var response = await _client.PostAsync("/api/log/analyze", content);

            // Should NOT crash — middleware handles and returns an ApiError object
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Swagger_Should_Load()
        {
            var response = await _client.GetAsync("/swagger/index.html");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
