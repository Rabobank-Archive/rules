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


            securityReport.ApplicationGroupContainsProductionEnvironmentOwner.ShouldBeTrue();
            securityReport.ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup.ShouldBeTrue();
            
            securityReport.BuildRightsBuildAdmin.BuildRightsIsSecure.ShouldBeTrue();
            securityReport.BuildRightsProjectAdmin.BuildRightsIsSecure.ShouldBeTrue();
            securityReport.RepositoryRightsProjectAdmin.RepositoryRightsIsSecure.ShouldBeTrue();
            
            securityReport.ProjectIsSecure.ShouldBeTrue();
        }

        [Fact]
        public void SecurityReportScanExecuteWithApplicationsGroupsResponseShouldNotBeEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var applicationGroup1 = new Response.ApplicationGroup {DisplayName = "[dummy]\\Project Administrators", TeamFoundationId = "1234",};
            var applicationGroup2 = new Response.ApplicationGroup {DisplayName = "[TAS]\\Rabobank Project Administrators"};
            var applicationGroup3 = new Response.ApplicationGroup {DisplayName = "[dummy]\\Build Administrators", TeamFoundationId = "4321",};
            var applicationGroups = new Response.ApplicationGroups {Identities = new[] {applicationGroup1 , applicationGroup2, applicationGroup3}};

            var securityNamespace1 = new Response.SecurityNamespace {DisplayName = "Git Repositories", NamespaceId = "123456"};
            var securityNamespace2 = new Response.SecurityNamespace {Name = "Build", NamespaceId = "54321"};
            var securityNamespaces = new Response.Multiple<Response.SecurityNamespace>(securityNamespace1, securityNamespace2);
            

            var client = Substitute.For<IVstsRestClient>();

            client.Get(Arg.Any<IVstsRestRequest<Response.ApplicationGroups>>()).Returns(applicationGroups);
            
            client.Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.SecurityNamespace>>>()).Returns(securityNamespaces);
            client.Get(Arg.Any<IVstsRestRequest<Response.ProjectProperties>>()).Returns(fixture.Create<Response.ProjectProperties>());

            client.Get(Arg.Any<IVstsRestRequest<Response.Multiple<Response.Repository>>>()).Returns(fixture.Create<Response.Multiple<Response.Repository>>());

            client.Get(Arg.Any<IVstsRestRequest<Response.PermissionsSetId>>()).Returns(fixture.Create<Response.PermissionsSetId>());

            var scan = new SecurityReportScan(client);
            var securityReport = scan.Execute("dummy");

            securityReport.ShouldNotBeNull();
        }
        
        
        [Fact]
        public void EmptyRepositoryRightsProjectAdminShouldBeFalse()
        {
            var report = new SecurityReport
            {
                BuildRightsBuildAdmin = new BuildRights(),
                BuildRightsProjectAdmin = new BuildRights(),
                RepositoryRightsProjectAdmin = new RepositoryRights(),
            };
            
            report.RepositoryRightsProjectAdmin.RepositoryRightsIsSecure.ShouldBeFalse();
        }
        
        [Fact]
        public void EmptyBuildRightsBuildAdminShouldBeFalse()
        {
            var report = new SecurityReport
            {
                BuildRightsBuildAdmin = new BuildRights(),
                BuildRightsProjectAdmin = new BuildRights(),
                RepositoryRightsProjectAdmin = new RepositoryRights(),
            };
            
            report.BuildRightsBuildAdmin.BuildRightsIsSecure.ShouldBeFalse();
        }

        
        
        [Theory]
        [InlineData(
            true, true, 
            true, true, true, true, true,
            true, true, true, true, true,
            true, true, true, true, true,
            true)]
        [InlineData(
            false, true, 
            true, true, true, true, true,
            true, true, true, true, true,
            true, true, true, true, true,
            false)]
        [InlineData(
            true, false, 
            true, true, true, true, true,
            true, true, true, true, true,
            true, true, true, true, true,
            false)]
        [InlineData(
            false, false, 
            true, true, true, true, true,
            true, true, true, true, true,
            true, true, true, true, true,
            false)]
        [InlineData(
            true, true, 
            false, true, true, true, false,
            true, true, true, true, true,
            true, true, true, true, true,
            false)]       
        [InlineData(
            true, true, 
            true, true, true, true, true,
            false, true, true, true, false,
            true, true, true, true, true,
            false)]       
        [InlineData(
            true, true, 
            true, true, true, true, true,
            true, true, true, true, true,
            false, true, true, true, false,
            false)]       
        public void CheckBuildRightsProjectAdmin(
            bool a, bool b,
            bool aa, bool ab, bool ac, bool ad, bool ae,
            bool ba, bool bb, bool bc, bool bd, bool be,
            bool ca, bool cb, bool cc, bool cd, bool ce,
            bool isSecure)
        {
            //Arrange
            var report = new SecurityReport
            {
                ApplicationGroupContainsProductionEnvironmentOwner = a,
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = b,
                
                BuildRightsBuildAdmin = new BuildRights
                {
                    HasNoPermissionsToDeleteBuilds = aa,
                    HasNoPermissionsToDeDestroyBuilds = ab,
                    HasNoPermissionsToDeleteBuildDefinition = ac,
                    HasNoPermissionsToAdministerBuildPermissions = ad
                },
                BuildRightsProjectAdmin = new BuildRights
                {
                    HasNoPermissionsToDeleteBuilds = ba,
                    HasNoPermissionsToDeDestroyBuilds = bb,
                    HasNoPermissionsToDeleteBuildDefinition = bc,
                    HasNoPermissionsToAdministerBuildPermissions = bd
                },

                RepositoryRightsProjectAdmin = new RepositoryRights
                {
                    HasNoPermissionToManagePermissionsRepositories = ca,
                    HasNoPermissionToManagePermissionsRepositorySet = cb,
                    HasNoPermissionToDeleteRepositorySet = cc,
                    HasNoPermissionToDeleteRepositories = cd
                },
            };
            
            //Act & Assert
            report.BuildRightsProjectAdmin.BuildRightsIsSecure.Equals(be).ShouldBeTrue();
            report.BuildRightsBuildAdmin.BuildRightsIsSecure.Equals(ae).ShouldBeTrue();
            report.RepositoryRightsProjectAdmin.RepositoryRightsIsSecure.Equals(ce).ShouldBeTrue();
            report.ProjectIsSecure.Equals(isSecure).ShouldBeTrue();
        }
    }
}