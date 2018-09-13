
using System;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.Rules.Release;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    /// <summary>
    /// This is a test
    /// </summary>
    public class ScanTests : IClassFixture<TestConfig>
    {
        private const string Project = "SOx-compliant-demo";
        private readonly ITestOutputHelper output;

        public ScanTests(ITestOutputHelper output, TestConfig config)
        {
            this.output = output;
            Config = config;
        }

        public TestConfig Config { get; }

        [Fact]
        public void GetAllRules()
        {
            var organization = Config.Organization;
            string token = Config.Token;

            var client = new VstsRestClient(organization, token);
            var scan = new Scan(client, Console.WriteLine);
            scan.Execute(Config.Project);
        }

        [Fact]
        public void Test714()
        {
            var client = new VstsRestClient(Config.Organization, Config.Token);
            var rule =  new FourEyesOnAllBuildArtefacts();
            
            var release = client.Execute(new VstsRestRequest<Response.Release>("https://somecompany.vsrm.visualstudio.com/f64ffdfa-0c4e-40d9-980d-bb8479366fc5/_apis/Release/releases/741", Method.GET));
            rule.GetResult(release.Data, 1915).ShouldBeTrue();
        }

        [Fact]
        public void ThrowsOnErrorWhenServiceEndpointsFails()
        {
            var client = Substitute.For<IVstsRestClient>();
            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>()).Returns(new RestResponse<Response.Multiple<Response.ServiceEndpoint>>{
                ErrorMessage = "fail"
            });

            var scan = new Scan(client, _ => {});
            var ex = Assert.Throws<Exception>(() => scan.Execute("dummy"));
            ex.Message.ShouldBe("fail");
        }

        [Fact]
        public void ThrowsOnErrorWhenServiceEndpointHistoryFails()
        {
            var client = Substitute.For<IVstsRestClient>();

            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpoint>>>()).Returns(new RestResponse<Response.Multiple<Response.ServiceEndpoint>>{
                Data = new Response.Multiple<Response.ServiceEndpoint> {
                    Value = new [] {
                        new Response.ServiceEndpoint {
                        }
                    }
                }
            });

            client.Execute(Arg.Any<IVstsRestRequest<Response.Multiple<Response.ServiceEndpointHistory>>>()).Returns(new RestResponse<Response.Multiple<Response.ServiceEndpointHistory>>{
                ErrorMessage = "fail"
            });

            var scan = new Scan(client, _ => {});
            var ex = Assert.Throws<Exception>(() => scan.Execute("dummy"));
            ex.Message.ShouldBe("fail");
        }

    }
}
