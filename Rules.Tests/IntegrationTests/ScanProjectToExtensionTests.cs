using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Xunit;
using Xunit.Abstractions;
using static SecurePipelineScan.VstsService.Requests.ExtensionManagement;

namespace SecurePipelineScan.Rules.Tests.IntegrationTests
{
    public class ScanProjectToExtensionTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper _output;
        private readonly TestConfig _config;
        private readonly VstsRestClient _client;

        public ScanProjectToExtensionTests(ITestOutputHelper output, TestConfig config)
        {
            _output = output;
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public void ScanProjectToExtensionsData()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);

            string projectName = _config.Project;

            var scan = new SecurityReportScan(client);
            var report = scan.Execute(projectName);

            var extensionData = report.Map();

            string extensionName = _config.ExtensionName;

            _client.Put(ExtensionData<ProjectOverviewData>("tas", extensionName,
                "ProjectOverview"), extensionData);
        }
    }
}