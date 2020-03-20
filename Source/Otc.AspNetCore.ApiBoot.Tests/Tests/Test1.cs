using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Otc.AspNetCore.ApiBoot.TestHost;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Otc.AspNetCore.ApiBoot.Tests.Tests
{
    public class Test1 : IClassFixture<HttpFixture<Startup>>
    {
        private const string apiVersion = "1.0";
        private readonly HttpFixture<Startup> httpFixture;
        private readonly HttpClient httpClient;

        public Test1(HttpFixture<Startup> httpFixture)
        {
            this.httpFixture = httpFixture ?? 
                throw new ArgumentNullException(nameof(httpFixture));

            httpClient = httpFixture.CreateServer(services => 
            {
                services.AddSingleton<ILoggerFactory>(new TestLoggerFactory());
            }).CreateClient();
            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, $"v{apiVersion}/");

        }

        [Fact]
        public async Task Test_Get()
        {
            var response = await httpClient.GetAsync("Tests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task Test_Exception()
        {
            var response = await httpClient.GetAsync("Tests/Exception");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var content = response.Content.ReadAsStringAsync();
        }
    }
}
