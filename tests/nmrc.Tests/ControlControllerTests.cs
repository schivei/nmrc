using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Nmrc.Tests
{
    public class ControlControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ControlControllerTests()
        {
            _server = new TestServer(new WebHostBuilder().ConfigureAppConfiguration(builder =>
                builder.AddJsonFile("appsettings.json").AddEnvironmentVariables())
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private const string BAD = "Response status code does not indicate success: 400 (Bad Request).";

        [Theory(DisplayName = "Move robot commands tests")]
        [InlineData("MMRMMRMM", "(2, 0, S)", false)]
        [InlineData("MML", "(0, 2, W)", true)]
        [InlineData("AAA", BAD, false)]
        [InlineData("MMMMMMMMMMMMMMMMMMMMMMMM", BAD, false)]
        [InlineData("RMMMMMMMMMMMMMMMMMMMMMMM", BAD, false)]
        [InlineData("LMMMMMMMMMMMMMMMMMMMMMMM", BAD, false)]
        [InlineData("LM", BAD, false)]
        [InlineData("RRM", BAD, false)]
        public async Task TestCommands(string command, string expected, bool repeat)
        {
            try
            {
                var response = await _client.GetAsync($"/rest/mars/{command}");
                response = response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                Assert.Equal(expected, content);
            }
            catch (HttpRequestException hre)
            {
                Assert.Equal(expected, hre.Message);
            }

            if (repeat)
                await TestCommands(command, expected, false);
        }
    }
}
