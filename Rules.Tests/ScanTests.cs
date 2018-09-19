
using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.Rules.Release;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;
using SecurePipelineScan.Rules.Reports;

namespace SecurePipelineScan.Rules.Tests
{
    public class ScanTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper output;
        private readonly TestConfig config;

        public ScanTests(ITestOutputHelper output, TestConfig config)
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
            var scan = new Scan(client, x => output.WriteLine($"{x.Request.Definition.Name}: {(x as ReleaseReport)?.Result}"));
            scan.Execute(config.Project);
        }

        [Fact]
        public void ThrowsOnErrorWhenServiceEndpointsFails()
        {
            var client = Substitute.For<IVstsRestClient>();
            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>()).Returns(new RestResponse<Response.Multiple<Response.ServiceEndpoint>>
            {
                ErrorMessage = "fail"
            });

            var scan = new Scan(client, _ => { });
            var ex = Assert.Throws<Exception>(() => scan.Execute("dummy"));
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ThrowsOnErrorWhenServiceEndpointHistoryFails()
        {
            var client = Substitute.For<IVstsRestClient>();

            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>()).Returns(new RestResponse<Response.Multiple<Response.ServiceEndpoint>>
            {
                Data = new Response.Multiple<Response.ServiceEndpoint>
                {
                    Value = new[] {
                        new Response.ServiceEndpoint {
                        }
                    }
                }
            });

            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>>>()).Returns(new RestResponse<Response.Multiple<Response.ServiceEndpointHistory>>
            {
                ErrorMessage = "fail"
            });

            var scan = new Scan(client, _ => { });
            var ex = Assert.Throws<Exception>(() => scan.Execute("dummy"));
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ThrowsOnErrorWhenReleaseFails()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            fixture.Customize<RestResponse<Response.Multiple<Response.ServiceEndpoint>>>(
                e => e.Without(x => x.ErrorMessage));

            fixture.Customize<RestResponse<Response.Multiple<Response.ServiceEndpointHistory>>>(
                e => e.Without(x => x.ErrorMessage));

            fixture.Customize<Response.ServiceEndpointHistoryData>(
                e => e.With(x => x.PlanType, "Release"));

            var endpoints = fixture.Create<RestResponse<Response.Multiple<Response.ServiceEndpoint>>>();
            var history = fixture.Create<RestResponse<Response.Multiple<Response.ServiceEndpointHistory>>>();
            var release = fixture.Build<RestResponse<Response.Release>>().With(
                e => e.ErrorMessage, "fail").Create();

            var client = Substitute.For<IVstsRestClient>();
            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>())
                .Returns(endpoints);

            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>>>())
                .Returns(history);

            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Release>>())
                .Returns(release);


            var scan = new Scan(client, _ => { });
            var ex = Assert.Throws<Exception>(() => scan.Execute("dummy"));
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ReportsProgress()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            fixture.Customize<RestResponse<Response.Multiple<Response.ServiceEndpoint>>>(
                e => e.Without(x => x.ErrorMessage));

            fixture.Customize<RestResponse<Response.Multiple<Response.ServiceEndpointHistory>>>(
                e => e.Without(x => x.ErrorMessage));

            fixture.Customize<RestResponse<Response.Release>>(
                e => e.Without(x => x.ErrorMessage));

            fixture.Customize<Response.ServiceEndpointHistoryData>(
                e => e.With(x => x.PlanType, "Release"));

            var endpoints = fixture.Create<RestResponse<Response.Multiple<Response.ServiceEndpoint>>>();
            var history = fixture.Create<RestResponse<Response.Multiple<Response.ServiceEndpointHistory>>>();
            var release = fixture.Create<RestResponse<Response.Release>>();

            var client = Substitute.For<IVstsRestClient>();
            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>())
                .Returns(endpoints);

            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>>>())
                .Returns(history);

            client
                .Execute(Arg.Any<IVstsRestRequest<Response.Release>>())
                .Returns(release);


            var progress = Substitute.For<Action<ScanReport>>();
            var scan = new Scan(client, progress);
            scan.Execute("dummy");

            progress.Received().Invoke(Arg.Any<ScanReport>());
        }
    }
}
