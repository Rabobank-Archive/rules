using System;
using Xunit;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ServiceEndpoint : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _vsts;

        public ServiceEndpoint(TestConfig config)
        {
            _config = config;
            _vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryServiceConnections()
        {
            var endpoints = _vsts.Get(Requests.ServiceEndpoint.Endpoints(_config.Project)).ToList();
            endpoints.ShouldNotBeEmpty();

            var endpoint = endpoints.First();
            endpoint.Name.ShouldNotBeNullOrEmpty();
            endpoint.Id.ShouldNotBe(Guid.Empty);
            endpoint.Type.ShouldNotBeNullOrEmpty();
            endpoint.Url.ToString().ShouldNotBeNullOrEmpty();
        }

        [Fact]

        public async Task CreateEndpoint()
        {
            var endpoint = await _vsts.PostAsync(Requests.ServiceEndpoint.Endpoint(_config.Project), new Response.ServiceEndpoint
            {
                Name = "Test105",
                Type = "generic",
                Url = new Uri ("https://localhost"),
                Authorization = new Response.UsernamePassword("", null)
            });
            await _vsts.DeleteAsync(Requests.ServiceEndpoint.Endpoint(_config.Project, endpoint.Id));
        }
    }
}
