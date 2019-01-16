using System;
using Rules.Reports;
using SecurePipelineScan.Rules.Reports;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class BuildRightsTests
    {
        [Theory]
        [InlineData(true, true, true, true, true)]
        [InlineData(false, true, true, true, false)]
        [InlineData(true, false, true, true, false)]
        [InlineData(true, true, false, true, false)]
        [InlineData(true, true, true, false, false)]
        public void CheckBuildRights(bool a, bool b, bool c, bool d, bool e)
        {
            var buildRights = new BuildAdminBuildRights
            {
                HasNoPermissionsToDeleteBuilds = a,
                HasNoPermissionsToDestroyBuilds = b,
                HasNoPermissionsToDeleteBuildDefinition = c,
                HasNoPermissionsToAdministerBuildPermissions = d,
            };

            buildRights.IsSecure.ShouldBe(e);
            
        }
        
        [Fact]
        public void EmptyBuildRightsBuildAdminShouldBeFalse()
        {
            var report = new SecurityReport(DateTime.Now)
            {
                BuildRightsBuildAdmin = new BuildAdminBuildRights(),
                BuildRightsProjectAdmin = new ProjectAdminBuildRights(),
                RepositoryRightsProjectAdmin = new RepositoryRights(),
            };
            
            report.BuildRightsBuildAdmin.IsSecure.ShouldBeFalse();
        }
    }
}