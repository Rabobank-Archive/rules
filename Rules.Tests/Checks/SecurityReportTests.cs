using Rules.Reports;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class SecurityReportTests
    {
        [Theory]
        [InlineData(true, true, true, true, true, true)]
        [InlineData(false, true, true, true, true, false)]
        [InlineData(true, false, true, true, true, false)]
        [InlineData(true, true, false, true, true, false)]
        [InlineData(true, true, true, false, true, false)]
        [InlineData(true, true, true, true, false, false)]
        public void CheckSecurityReport(bool a, bool b, bool c, bool d, bool e, bool expected)
        {
            var securityReport = new SecurityReport
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
            };
            
            securityReport.ProjectIsSecure.ShouldBe(expected);

        }

    }
}