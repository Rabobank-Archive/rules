using SecurePipelineScan.Rules;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Requests = SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Diagnostics;
using System.Net;
using RestSharp;
using Xunit;

namespace VstsService.Tests
{
    [Trait("category", "integration")]
    public class Projects : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public Projects(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        /// <summary>
        /// Test if all projects have a Name
        /// </summary>
        [Fact]
        public void QueryProjects()
        {
            var definitions = client.Get(Requests.Project.Projects());
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }
    }
}