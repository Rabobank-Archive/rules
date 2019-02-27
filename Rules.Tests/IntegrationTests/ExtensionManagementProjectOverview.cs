using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace SecurePipelineScan.Rules.Tests.IntegrationTests
{
    public class ExtensionManagementProjectOverview : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper _output;
        private readonly TestConfig _config;
        private readonly VstsRestClient _client;

        public ExtensionManagementProjectOverview(ITestOutputHelper output, TestConfig config)
        {
            _output = output;
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public void PutShouldInsertDemoDataProjectOverview()
        {
            _client.Put(
                VstsService.Requests.ExtensionManagement.ExtensionData<ProjectOverviewData>("tas", _config.ExtensionName,
                "CompliancyOverview"), new ProjectOverviewData
                {
                    Id = "SOx-compliant-demo",
                    Reports = new List<ReportRow> {
                        new ReportRow() {
                            @namespace = "GlobalPermissions",
                            PermissionBit = 1,
                            permission = "Permanently Delete Work Items",
                            actualValue = 3,
                            shouldBe = 1,
                            IsCompliant = false,
                            Level = "",
                            applicationGroupName = "Production Environment Owner",
                            }
                        }
                });
        }

        private class ProjectOverviewData : ExtensionData
        {
            public IList<ReportRow> Reports { get; set; }
        }

        public class ReportRow
        {
            public string @namespace { get; internal set; }

            internal bool IsCompliant;
            internal string Level;
            public int PermissionBit { get; internal set; }
            public string permission { get; internal set; }
            public int actualValue { get; internal set; }
            public int shouldBe { get; internal set; }
            public string applicationGroupName { get; internal set; }
        }
    }
}