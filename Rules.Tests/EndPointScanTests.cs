using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Shouldly;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class EndPointScanTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper output;
        private readonly TestConfig config;

        public EndPointScanTests(ITestOutputHelper output, TestConfig config)
        {
            this.output = output;
            this.config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public void IntegrationTestOnScan()
        {
            var organization = config.Organization;
            var token = config.Token;

            var client = new VstsRestClient(organization, token);
            var scan = new EndPointScan(client);
            scan.Execute(config.Project).ToList().ForEach(x => output.WriteLine($"{x.Request.Definition.Name}: {(x as ReleaseReport)?.Result}"));
        }

        [Fact]
        public void ReportsProgress()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            fixture.Customize<Response.ServiceEndpointHistoryData>(
                e => e.With(x => x.PlanType, "Release"));

            var endpoints = fixture.Create<Response.Multiple<Response.ServiceEndpoint>>();
            var history = fixture.Create<Response.Multiple<Response.ServiceEndpointHistory>>();
            var release = fixture.Create<Response.Release>();

            var client = Substitute.For<IVstsRestClient>();
            client
                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>())
                .Returns(endpoints);

            client
                .Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>>>())
                .Returns(history);

            client
                .Get(Arg.Any<IVstsRestRequest<Response.Release>>())
                .Returns(release);

            var scan = new EndPointScan(client);
            scan.Execute("dummy").ShouldNotBeEmpty();
        }
    }
}