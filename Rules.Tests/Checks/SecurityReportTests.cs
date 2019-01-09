using System;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class SecurityReportTests
    {
        [Theory]

        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true)]
        [InlineData(false, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, false, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, false, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, false, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, false, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, false, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, false, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, false, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, false, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, false, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, false, false)]
        public void CheckSecurityReport(
            bool a, bool b, bool c, bool d, bool e, bool f,bool g,bool h, bool i,  bool j, bool k, bool expected)
        {
            var securityReport = new SecurityReport(DateTime.Now)
            {
                ApplicationGroupContainsProductionEnvironmentOwner = a,
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = b,

                BuildRightsBuildAdmin = new BuildRights
                {
                    HasNoPermissionsToDeleteBuilds = c,
                    HasNoPermissionsToDeDestroyBuilds = c,
                    HasNoPermissionsToDeleteBuildDefinition = c,
                    HasNoPermissionsToAdministerBuildPermissions = c
                },
                BuildRightsProjectAdmin = new BuildRights
                {
                    HasNoPermissionsToDeleteBuilds = d,
                    HasNoPermissionsToDeDestroyBuilds = d,
                    HasNoPermissionsToDeleteBuildDefinition = d,
                    HasNoPermissionsToAdministerBuildPermissions = d
                },

                RepositoryRightsProjectAdmin = new RepositoryRights
                {
                    HasNoPermissionToManagePermissionsRepositories = e,
                    HasNoPermissionToManagePermissionsRepositorySet = e,
                    HasNoPermissionToDeleteRepositorySet = e,
                    HasNoPermissionToDeleteRepositories = e
                },
                
                BuildDefinitionsRightsBuildAdmin = new BuildRights
                {
                    HasNoPermissionsToDeleteBuilds = f,
                    HasNoPermissionsToDeDestroyBuilds = f,
                    HasNoPermissionsToDeleteBuildDefinition = f,
                    HasNoPermissionsToAdministerBuildPermissions = f
                },
                
                BuildDefinitionsRightsProjectAdmin = new BuildRights
                {
                    HasNoPermissionsToDeleteBuilds = g,
                    HasNoPermissionsToDeDestroyBuilds = g,
                    HasNoPermissionsToDeleteBuildDefinition = g,
                    HasNoPermissionsToAdministerBuildPermissions = g
                },
                
                ReleaseRightsContributor = new ReleaseRights
                {
                    HasNoPermissionToCreateReleases = h,
                    HasNoPermissionToDeleteReleases = h,
                    HasNoPermissionToManageReleaseApprovers = h,
                    HasNoPermissionToDeleteReleasePipeline = h,
                    HasNoPermissionsToAdministerReleasePermissions = h
                },
               
                ReleaseRightsRaboProjectAdmin = new ReleaseRights
                {
                    HasNoPermissionToCreateReleases = i,
                    HasNoPermissionToDeleteReleases = i,
                    HasNoPermissionToManageReleaseApprovers = i,
                    HasNoPermissionToDeleteReleasePipeline = i,
                    HasNoPermissionsToAdministerReleasePermissions = i
                },
                
                ReleaseRightsProductionEnvOwner = new ReleaseRightsProductionEnvOwner
                {
                    HasNoPermissionToCreateReleases = j,
                    HasPermissionToManageReleaseApprovers = j
                },

                TeamRabobankProjectAdministrators = new GlobalRights
                {
                    HasNoPermissionToDeleteTeamProject = k,
                    HasNoPermissionToPermanentlyDeleteWorkitems = k,
                    HasNoPermissionToManageProjectProperties = k
                }

            };
            
            securityReport.ProjectIsSecure.ShouldBe(expected);
        }

    }
}