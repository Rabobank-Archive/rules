using System;
using System.Net.Http;
using Flurl.Http.Testing;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;

namespace LogAnalytics.Client.Tests
{
    public class LogAnalyticsClientTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public LogAnalyticsClientTests(TestConfig config)
        {
            _config = config;
        }
        
        [Fact]
        public void HeadersNotSetMultipleTimesWhenClientIsUsedInParallel()
        {
            var sut = new LogAnalyticsClient("adsf", "", Substitute.For<IAzureTokenProvider>());
            using (new HttpTest())
            {
                Parallel.For(0, 100, async
                    x => await sut.AddCustomLogJsonAsync("", "", ""));
            }
        }

        [Fact]
        public async Task TestRequiredHeadersForPostToLogAnalytics()
        {
            var client = new LogAnalyticsClient("sdpfj", "", Substitute.For<IAzureTokenProvider>());
            using var test = new HttpTest();
            await client.AddCustomLogJsonAsync("bla", "", "asdf");
            test
                .ShouldHaveCalled("https://sdpfj.ods.opinsights.azure.com/api/logs?api-version=2016-04-01")
                .WithHeader("Authorization")
                .WithHeader("Log-Type", "bla")
                .WithHeader("x-ms-date")
                .WithHeader("time-generated-field", "asdf")
                .WithContentType("application/json");
        }

        [Fact]
        public async Task QueryRequestShouldIncludeBearerToken()
        {
            var tokenProvider = Substitute.For<IAzureTokenProvider>();
            tokenProvider.GetAccessTokenAsync().Returns("dummyToken");
            var client = new LogAnalyticsClient("sdpfj", "", tokenProvider);

            using var httpTest = new HttpTest();
            await client.QueryAsync("some query");
            httpTest.ShouldHaveMadeACall().WithOAuthBearerToken("dummyToken");
        }
        
        [Fact]
        public async Task QueryRequestShouldIncludeQuery()
        {
            var tokenProvider = Substitute.For<IAzureTokenProvider>();
            tokenProvider.GetAccessTokenAsync().Returns("dummyToken");
            var client = new LogAnalyticsClient("sdpfj", "", tokenProvider);

            using var httpTest = new HttpTest();
            await client.QueryAsync("some query");
            httpTest.ShouldHaveMadeACall()
                .WithVerb(HttpMethod.Post)
                .WithRequestJson(new LogAnalyticsQuery { query = "some query" });
        }

        [Fact]
        public async Task QueryShouldReturnResults()
        {
            var tokenprovider = new AzureTokenProvider(_config.TenantId, _config.ClientId, _config.ClientSecret);
            var client = new LogAnalyticsClient(_config.Workspace, _config.Key, tokenprovider);
            await client.AddCustomLogJsonAsync("preventive_analysis_log", new {Date = DateTime.UtcNow, Id=1}, "Date");
            var result = await client.QueryAsync("preventive_analysis_log_CL | limit 50");
            result.ShouldNotBeNull();
            result.tables.ShouldNotBeEmpty();
            result.tables[0].columns.ShouldNotBeEmpty();
            result.tables[0].rows.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task QueryShouldReturnNullForBadQuery()
        {
            var tokenprovider = new AzureTokenProvider(_config.TenantId, _config.ClientId, _config.ClientSecret);
            var client = new LogAnalyticsClient(_config.Workspace, _config.Key, tokenprovider);
            await client.AddCustomLogJsonAsync("preventive_analysis_log", new { Date = DateTime.UtcNow, Id = 1 }, "Date");
            var result = await client.QueryAsync("some query");
            result.ShouldBeNull();
        }
    }
}