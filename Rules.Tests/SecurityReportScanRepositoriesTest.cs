using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using RestSharp;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class SecurityReportScanRepositoriesTest : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper output;
        private readonly TestConfig config;

        public SecurityReportScanRepositoriesTest(ITestOutputHelper output, TestConfig config)
        {
            this.output = output;
            this.config = config;
        }

        [Fact]
        [Trait("Category", "integration")]
        public void IntegrationTestPermissions()
        {

            var client = new VstsRestClient(config.Organization, config.Token);
            var scan = new SecurityReportScanRepositories(client);
            var securityReport = scan.Execute(config.Project);
            
            securityReport.ProjectAdminHasNoPermissionsToDeleteRepositorySet.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionToManagePermissionsRepositorySet.ShouldBeTrue();
        }
    }
}