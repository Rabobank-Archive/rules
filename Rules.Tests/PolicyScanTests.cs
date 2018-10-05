using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using RestSharp;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Shouldly;
using System;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class PolicyScanTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper output;
        private readonly TestConfig config;

        public PolicyScanTests(ITestOutputHelper output, TestConfig config)
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
            var scan = new PolicyScan(client, x => output.WriteLine($"Branch Id: {(x as BranchPolicyReport)?.BranchPolicy.Id}"));
            scan.Execute(config.Project);
        }

        [Fact]
        public void ThrowsOnErrorWhenMinimumNumberOfReviewersPolicyFails()
        {
            var client = Substitute.For<IVstsRestClient>();
            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>>()).Returns(new RestResponse<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>
            {
                ErrorMessage = "fail"
            });

            var scan = new PolicyScan(client, _ => { });
            var ex = Assert.Throws<Exception>(() => scan.Execute("dummy"));
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ReportsProgress()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            fixture.Customize<RestResponse<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>>(
            e => e.Without(x => x.ErrorMessage));

            var minimumNumberOfReviewersPolicy = fixture.Create<RestResponse<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>>();

            var client = Substitute.For<IVstsRestClient>();
            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.MinimumNumberOfReviewersPolicy>>>())
                .Returns(minimumNumberOfReviewersPolicy);

            var progress = Substitute.For<Action<ScanReport>>();
            var scan = new PolicyScan(client, progress);
            scan.Execute("dummy");

            progress.Received().Invoke(Arg.Any<ScanReport>());
        }
    }
}