using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.VstsService;
using Shouldly;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class RepositoryScanTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper output;
        private readonly TestConfig config;

        public RepositoryScanTests(ITestOutputHelper output, TestConfig config)
        {
            this.output = output;
            this.config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public void IntegrationTestOnScan()
        {
            var organization = config.Organization;
            string token = config.Token;

            var client = new VstsRestClient(organization, token);
            var scan = new RepositoryScan(client);
            scan.Execute(config.Project).ToList().ForEach(x => output.WriteLine($"Repository: {x.Repository}, Result: {x.HasRequiredReviewerPolicy}"));
        }

        [Fact]
        public void ReportsProgress()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var minimumNumberOfReviewersPolicy = fixture.Create<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>();
            var repos = fixture.Create<Response.Multiple<Response.Repository>>();

            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>>())
                .Returns(minimumNumberOfReviewersPolicy);

            client
                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.Repository>>>())
                .Returns(repos);

            var scan = new RepositoryScan(client);
            scan.Execute("dummy").ShouldNotBeEmpty();
        }
    }
}