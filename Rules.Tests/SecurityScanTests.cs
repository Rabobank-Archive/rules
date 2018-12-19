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
    public class SecurityReportScanTest : IClassFixture<TestConfig>
    {
        private readonly ITestOutputHelper output;
        private readonly TestConfig config;

        public SecurityReportScanTest(ITestOutputHelper output, TestConfig config)
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
            var scan = new SecurityReportScan(client);
            var securityReport = scan.Execute(config.Project);
            
            securityReport.ApplicationGroupContainsProductionEnvironmentOwner.ShouldBeTrue();
            securityReport.ApplicationGroupContainsRabobankProjectAdministrators.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionsToDeleteRepositorySet.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionToManagePermissionsRepositorySet.ShouldBeTrue();

        }
        
        [Fact]
        public void SecurityReportScanExecuteWithApplicationsGroupsResponseShouldNotBeEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var applicationGroups = fixture.Create<Response.ApplicationGroups>();

            var client = Substitute.For<IVstsRestClient>();
            
            client.Get(Arg.Any<IVstsRestRequest<Response.ApplicationGroups>>()).Returns(applicationGroups);


            var scan = new SecurityReportScan(client);
            var securityReport = scan.Execute("dummy");
            
            securityReport.ShouldNotBeNull();
        }

    }
}