using System;
using Xunit;
using NineNineQuotes.Controllers;
using NineNineQuotes.Services;
using Moq;
using Microsoft.AspNetCore.Mvc.Testing;
using NineNineQuotes;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using NineNineQuotes.Data;

namespace NineNineTests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _fixture;
        private readonly HttpClient _httpClient;
       
        public IntegrationTests(WebApplicationFactory<Startup> fixture)
        {
            _fixture = fixture;
            _httpClient = _fixture.CreateClient();
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<HttpResponseMessage> CreateRequest(HttpMethod method, string url)
        {
            HttpRequestMessage request = new(method, url);
            request.Headers.Add("X-ClientId", "dev-id-1");
            request.Headers.Add("X-Real-IP", "127.0.0.1");
            return await _httpClient.SendAsync(request);
        }

        [Fact]
        public async Task GetRandomQuote_HappyPath()
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), "api/quotes/random");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GetRandomQuote_ErrorPath()
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("POST"), "api/quotes/random");
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/random/from?character=Amy")]
        [InlineData("api/quotes/random/from?episode=AC/DC")]
        [InlineData("api/quotes/random/from?character=Amy&episode=AC/DC")]
        [InlineData("api/quotes/random/from?character=&episode=AC/DC")]
        public async Task GetRandomQuoteFromAsync_HappyPath(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);            
        }

        [Theory]
        [InlineData("api/quotes/random/from?character=A")]
        [InlineData("api/quotes/random/from?episode=3")]
        [InlineData("api/quotes/random/from?character=A&episode=3")]
        [InlineData("api/quotes/random/from?character=&episode=")]
        [InlineData("api/quotes/random/from?characer=&episde=")]
        public async Task GetRandomQuoteFromAsync_EdgeCases(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/all")]
        [InlineData("api/quotes/all?PageNumber=1&PageSize=50")]
        [InlineData("api/quotes/all?pageNumber=2&pageSize=50")]
        [InlineData("api/quotes/all?pageNumber=0&pageSize=50")]
        [InlineData("api/quotes/all?pageNumber=1&pageSize=100")]
        [InlineData("api/quotes/all?pageNumber=-1&pageSize=100")]
        [InlineData("api/quotes/all?pageNumber=1&pageSize=-2")]
        [InlineData("api/quotes/all?pageNumber=1&pageSize=0")]
        public async Task GetAllQuotesAsync_HappyPath(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/all?PageNumber=&PageSize=")]
        [InlineData("api/quotes/all?pageNumber=a&pageSize=b")]
        public async Task GetAllQuotesAsync_ErrorPath(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/all/from?character=Jake&PageNumber=1&PageSize=25")]
        [InlineData("api/quotes/all/from?episode=AC/DC&PageNumber=1&PageSize=25")]
        [InlineData("api/quotes/all/from?character=Jake&episode=AC/DC&PageNumber=1&PageSize=25")]
        public async Task GetAllQuotesFromAsync_HappyPath(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/all/from?character=Car&PageNumber=1&PageSize=25")]
        [InlineData("api/quotes/all/from?episode=ad&PageNumber=1&PageSize=25")]
        [InlineData("api/quotes/all/from?character=Car&episode=ad&PageNumber=1&PageSize=25")]
        [InlineData("api/quotes/all/from?character=-1&PageNumber=1&PageSize=25")]
        [InlineData("api/quotes/all/from?episode=-1&PageNumber=1&PageSize=25")]
        public async Task GetAllQuotesFromAsync_EdgeCases(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/find?searchTerm=pet")]
        [InlineData("api/quotes/find?character=Amy&searchTerm=pet")]
        [InlineData("api/quotes/find?character=Amy&searchTerm=pet&PageNumber=1&PageSize=25")]
        public async Task FindQuoteFromAsync_HappyPath(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("api/quotes/find?searchTerm=s91u")]
        [InlineData("api/quotes/find?character=5ss2&searchTerm=s91u")]
        public async Task FindQuoteFromAsync_EdgeCases(string url)
        {
            HttpResponseMessage response = await CreateRequest(new HttpMethod("GET"), url);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
