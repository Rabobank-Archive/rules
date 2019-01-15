using System;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class RepositoryRightsTests
    {
        [Theory]
        [InlineData(true, true, true, true, true)]
        [InlineData(false, true, true, true, false)]
        [InlineData(true, false, true, true, false)]
        [InlineData(true, true, false, true, false)]
        [InlineData(true, true, true, false, false)]
        public void CheckRepositoryRights(bool a, bool b, bool c, bool d, bool e)
        {
            var repositoryRights = new RepositoryRights
            {
                HasNotSetToManagePermissionsRepositories = a,
                HasNotSetToManagePermissionsRepositorySet = b,
                HasNoPermissionToDeleteRepositorySet = c,
                HasNoPermissionToDeleteRepositories = d
            };
            
            repositoryRights.RepositoryRightsIsSecure.Equals(e).ShouldBeTrue();
        }
        
        [Fact]
        public void EmptyRepositoryRightsProjectAdminShouldBeFalse()
        {
            var report = new SecurityReport(DateTime.Now)
            {
                BuildRightsBuildAdmin = new BuildAdminBuildRights(),
                BuildRightsProjectAdmin = new ProjectAdminBuildRights(),
                RepositoryRightsProjectAdmin = new RepositoryRights(),
            };
            
            report.RepositoryRightsProjectAdmin.RepositoryRightsIsSecure.ShouldBeFalse();
        }


    }
}