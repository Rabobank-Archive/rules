using System;
using NSubstitute.Core;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class SecurityReportTests
    {
        [Theory]

        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true)]
        [InlineData(false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, true, false)]
        [InlineData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false)]

        public void CheckSecurityReport(bool a, bool b, bool c, bool d, bool e, bool f, bool g, bool h, bool i, bool j,
            bool k, bool l, bool m, bool n, bool o, bool p, bool q, bool r, bool s, bool t, bool expected)
        {
            var securityReport = new SecurityReport(DateTime.Now)
            {
                ApplicationGroupContainsProductionEnvironmentOwner = a,
                ProjectAdminGroupOnlyContainsRabobankProjectAdminGroup = b,

                BuildRightsBuildAdmin = new BuildAdminBuildRights()
                {
                    HasNoPermissionsToDeleteBuilds = c,
                    HasNoPermissionsToDestroyBuilds = c,
                    HasNoPermissionsToDeleteBuildDefinition = c,
                    HasNoPermissionsToAdministerBuildPermissions = c
                },
                BuildRightsProjectAdmin = new ProjectAdminBuildRights()
                {
                    HasNoPermissionsToDeleteBuilds = d,
                    HasNoPermissionsToDestroyBuilds = d,
                    HasNoPermissionsToDeleteBuildDefinition = d,
                    HasNoPermissionsToAdministerBuildPermissions = d
                },

                RepositoryRightsProjectAdmin = new RepositoryRights()
                {
                    HasNotSetToManagePermissionsRepositories = e,
                    HasNotSetToManagePermissionsRepositorySet = e,
                    HasNoPermissionToDeleteRepositorySet = e,
                    HasNoPermissionToDeleteRepositories = e
                },
                
                BuildDefinitionsRightsBuildAdmin = new BuildAdminBuildRights()
                {
                    HasNoPermissionsToDeleteBuilds = f,
                    HasNoPermissionsToDestroyBuilds = f,
                    HasNoPermissionsToDeleteBuildDefinition = f,
                    HasNoPermissionsToAdministerBuildPermissions = f
                },
                
                BuildDefinitionsRightsProjectAdmin = new ProjectAdminBuildRights()
                {
                    HasNoPermissionsToDeleteBuilds = g,
                    HasNoPermissionsToDestroyBuilds = g,
                    HasNoPermissionsToDeleteBuildDefinition = g,
                    HasNoPermissionsToAdministerBuildPermissions = g
                },
                
                ReleaseRightsContributor = new ContributorsReleaseRights
                {
                    HasNoPermissionToCreateReleases = h,
                    HasNoPermissionToDeleteReleases = h,
                    HasNoPermissionToManageReleaseApprovers = h,
                    HasNoPermissionToDeleteReleasePipeline = h,
                    HasNoPermissionsToAdministerReleasePermissions = h,
                    HasNotSetToManageReleaseApprovers = h,
                    HasPermissionToCreateReleases = h,
                    HasPermissionToManageReleaseApprovers = h,
                    HasNotSetToDeleteReleaseStage =h,
                },
               
                ReleaseRightsRaboProjectAdmin = new RaboAdminReleaseRights
                {
                    HasNoPermissionToCreateReleases = i,
                    HasNoPermissionToDeleteReleases = i,
                    HasNoPermissionToManageReleaseApprovers = i,
                    HasNoPermissionToDeleteReleasePipeline = i,
                    HasNoPermissionsToAdministerReleasePermissions = i,
                    HasPermissionToManageReleaseApprovers = i,
                    HasPermissionToCreateReleases = i,
                    HasNotSetToManageReleaseApprovers = i,
                    HasNoPermissionToDeleteReleaseStage = i, 
                },
                
                ReleaseRightsProductionEnvOwner = new ProductionEnvOwnerReleaseRights
                {
                    HasNoPermissionToCreateReleases = j,
                    HasPermissionToManageReleaseApprovers = j,
                    HasPermissionToCreateReleases = j,
                    HasNoPermissionToDeleteReleases = j,
                    HasNotSetToManageReleaseApprovers = j,
                    HasNoPermissionToDeleteReleasePipeline = j,
                    HasNoPermissionToManageReleaseApprovers = j,
                    HasNoPermissionsToAdministerReleasePermissions = j,
                    HasPermissionToDeleteReleaseStage = j,
                },
                
                ReleaseDefinitionsRightsContributor = new ContributorsReleaseRights()
                {
                    HasNoPermissionToCreateReleases = k,
                    HasNoPermissionToDeleteReleases = k,
                    HasNoPermissionToDeleteReleasePipeline = k,
                    HasNoPermissionToManageReleaseApprovers = k,
                    HasNoPermissionsToAdministerReleasePermissions = k,
                    HasPermissionToCreateReleases = k,
                    HasPermissionToManageReleaseApprovers = k,
                    HasNotSetToManageReleaseApprovers = k,
                    HasNotSetToDeleteReleaseStage = k,
                },
                
                ReleaseDefinitionsRightsRaboProjectAdmin = new RaboAdminReleaseRights()
                {
                    HasNoPermissionToCreateReleases = l,
                    HasNoPermissionToDeleteReleases = l,
                    HasNoPermissionToDeleteReleasePipeline = l,
                    HasNoPermissionToManageReleaseApprovers = l,
                    HasNoPermissionsToAdministerReleasePermissions = l,
                    HasPermissionToCreateReleases = l,
                    HasPermissionToManageReleaseApprovers = l,
                    HasNotSetToManageReleaseApprovers = l,
                    HasNoPermissionToDeleteReleaseStage = l,
                },
                
                ReleaseDefinitionsRightsProductionEnvOwner = new ProductionEnvOwnerReleaseRights()
                {
                    HasNoPermissionToCreateReleases = m,
                    HasPermissionToManageReleaseApprovers = m,
                    HasPermissionToCreateReleases = m,
                    HasNoPermissionToDeleteReleases = m,
                    HasNotSetToManageReleaseApprovers = m,
                    HasNoPermissionToDeleteReleasePipeline = m,
                    HasNoPermissionToManageReleaseApprovers = m,
                    HasNoPermissionsToAdministerReleasePermissions = m,
                    HasPermissionToDeleteReleaseStage = m,
                },

                TeamRabobankProjectAdministrators = new GlobalRights
                {
                    HasNoPermissionToDeleteTeamProject = n,
                    HasNoPermissionToPermanentlyDeleteWorkitems = n,
                    HasNoPermissionToManageProjectProperties = n
                },
                
                BuildRightsContributor = new ContributorsBuildRights
                {
                    HasNoPermissionsToDeleteBuildDefinition = o, 
                    HasNoPermissionsToDeleteBuilds = o,
                    HasNoPermissionsToDestroyBuilds = o, 
                    HasNoPermissionsToAdministerBuildPermissions = o,
                },
                
                BuildDefinitionsRightsContributor = new ContributorsBuildRights
                {
                    HasNoPermissionsToDeleteBuildDefinition = p, 
                    HasNoPermissionsToDeleteBuilds = p,
                    HasNoPermissionsToDestroyBuilds = p, 
                    HasNoPermissionsToAdministerBuildPermissions = p,
                },
                
                ReleaseRightsProjectAdmin = new ProjectAdminReleaseRights
                {                    
                    HasNoPermissionsToAdministerReleasePermissions = q,
                    HasNoPermissionToDeleteReleasePipeline = q,
                    HasNoPermissionToDeleteReleaseStage = q,
                    HasNoPermissionToDeleteReleases = q,
                    HasNoPermissionToManageReleaseApprovers = q,
                },
                
                ReleaseDefinitionsRightsProjectAdmin = new ProjectAdminReleaseRights
                {                    
                    HasNoPermissionsToAdministerReleasePermissions = r,
                    HasNoPermissionToDeleteReleasePipeline = r,
                    HasNoPermissionToDeleteReleaseStage = r,
                    HasNoPermissionToDeleteReleases = r,
                    HasNoPermissionToManageReleaseApprovers = r,
                },
                
                ReleaseRightsReleaseAdmin = new ReleaseAdminReleaseRights
                {
                    HasNoPermissionsToAdministerReleasePermissions = s,
                    HasNoPermissionToDeleteReleasePipeline = s,
                    HasNoPermissionToDeleteReleases = s,
                    HasNoPermissionToManageReleaseApprovers =s,
                    HasNoPermissionToDeleteReleaseStage =s,
                },
                
                ReleaseDefinitionsRightsReleaseAdmin = new ReleaseAdminReleaseRights
                {
                    HasNoPermissionsToAdministerReleasePermissions = t,
                    HasNoPermissionToDeleteReleasePipeline = t,
                    HasNoPermissionToDeleteReleases = t,
                    HasNoPermissionToManageReleaseApprovers = t,
                    HasNoPermissionToDeleteReleaseStage = t,
                }
            };
            
            securityReport.ProjectIsSecure.ShouldBe(expected);
        }

    }
}