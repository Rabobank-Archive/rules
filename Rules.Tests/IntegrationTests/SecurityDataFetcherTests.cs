using SecurePipelineScan.Rules.Pdf;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace SecurePipelineScan.Rules.Tests.IntegrationTests
{
    public class SecurityDataFetcherTests : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper _output;
        private readonly TestConfig _config;

        public SecurityDataFetcherTests(ITestOutputHelper output, TestConfig config)
        {
            _output = output;
            _config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public void Integration_ProjectShouldHaveGroups()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var sut = new SecurityDataFetcher(client);

            string projectName = _config.Project;
            
            var result = sut.FetchSecurityPermissions(projectName);

            result.ProjectName.ShouldBe(projectName);
            result.GlobalPermissions.ShouldAllBe(x => !string.IsNullOrWhiteSpace(x.Key));

            // A minimum of 5 DevOps groups should be there.
            result.GlobalPermissions.Count.ShouldBeGreaterThan(5);
        }

        [Fact]
        [Trait("category", "integration")]
        public void Integration_CreatesPdf()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);

            string projectName = _config.Project;

            var scan = new SecurityReportScan(client);
            var report = scan.Execute(projectName);

            var pdf = new PdfService("Security_Reports");

            pdf.CreateReport(projectName, report);
        }
    }
}