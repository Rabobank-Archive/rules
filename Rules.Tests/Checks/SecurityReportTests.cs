using Rules.Reports;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class SecurityReportTests
    {
        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, true, false)]
        [InlineData(true, false, true, false)]
        public void CheckSecurityReport(bool a, bool b, bool c, bool d)
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
                    HasNoPermissionsToDeleteBuilds = c,
                    HasNoPermissionsToDeDestroyBuilds = c,
                    HasNoPermissionsToDeleteBuildDefinition = c,
                    HasNoPermissionsToAdministerBuildPermissions = c
                },

                RepositoryRightsProjectAdmin = new RepositoryRights
                {
                    HasNoPermissionToManagePermissionsRepositories = c,
                    HasNoPermissionToManagePermissionsRepositorySet = c,
                    HasNoPermissionToDeleteRepositorySet = c,
                    HasNoPermissionToDeleteRepositories = c
                },
            };
            
            securityReport.ProjectIsSecure.Equals(d).ShouldBeTrue();

        }

    }
}