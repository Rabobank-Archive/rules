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
using Requests = SecurePipelineScan.VstsService.Requests;

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

            securityReport.ProjectIsSecure.ShouldBeTrue();

            securityReport.ApplicationGroupContainsProductionEnvironmentOwner.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionToDeleteRepositorySet.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionToManagePermissionsRepositorySet.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionToManagePermissionsRepositories.ShouldBeTrue();
            securityReport.ProjectAdminHasNoPermissionToDeleteRepositories.ShouldBeTrue();
        }

        [Fact]
        public void SecurityReportScanExecuteWithApplicationsGroupsResponseShouldNotBeEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var applicationGroup1 = new Response.ApplicationGroup {DisplayName = "[dummy]\\Project Administrators", TeamFoundationId = "1234",};
            var applicationGroup2 = new Response.ApplicationGroup {DisplayName = "[TAS]\\Rabobank Project Administrators"};
            var applicationGroups = new Response.ApplicationGroups {Identities = new[] {applicationGroup1 , applicationGroup2}};

            var names = new Response.Multiple<Response.SecurityNamespace>(new Response.SecurityNamespace
            {
                DisplayName = "Git Repositories",
                NamespaceId = "123456"
            });


            var client = Substitute.For<IVstsRestClient>();

            client.Get(Arg.Any<IVstsRestRequest<Response.ApplicationGroups>>()).Returns(applicationGroups);
            
            client.Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.SecurityNamespace>>>()).Returns(names);
            client.Get(Arg.Any<IVstsRestRequest<Response.ProjectProperties>>()).Returns(fixture.Create<Response.ProjectProperties>());

            client.Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.Repository>>>()).Returns(fixture.Create<Response.Multiple<Response.Repository>>());

            client.Get(Arg.Any<IVstsRestRequest<Response.PermissionsRepository>>()).Returns(fixture.Create<Response.PermissionsRepository>());

            var scan = new SecurityReportScan(client);
            var securityReport = scan.Execute("dummy");

            securityReport.ShouldNotBeNull();
        }
    }
}