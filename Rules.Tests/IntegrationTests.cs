
using RestSharp;
using SecurePipelineScan.Rules.Release;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace SecurePipelineScan.Rules.Tests
{
    /// <summary>
    /// This is a test
    /// </summary>
    public class IntegrationTests : IClassFixture<TestConfig>
    {
        private const string Project = "SOx-compliant-demo";
        private readonly ITestOutputHelper output;

        public IntegrationTests(ITestOutputHelper output, TestConfig config)
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

            Scan.Execute(client, Config.Project);
        }

        [Fact]
        public void Test714()
        {
            var client = new VstsRestClient(Config.Organization, Config.Token);
            var rule =  new FourEyesOnAllBuildArtefacts();
            
            var release = client.Execute(new VstsRestRequest<SecurePipelineScan.VstsService.Response.Release>("https://somecompany.vsrm.visualstudio.com/f64ffdfa-0c4e-40d9-980d-bb8479366fc5/_apis/Release/releases/741", Method.GET));
            rule.GetResult(release.Data, 1915).ShouldBeTrue();
            
        }
    }
}
