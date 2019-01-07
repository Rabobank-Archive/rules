using Rules.Reports;
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
            var buildRights = new BuildRights
            {
                HasNoPermissionsToDeleteBuilds = a,
                HasNoPermissionsToDeDestroyBuilds = b,
                HasNoPermissionsToDeleteBuildDefinition = c,
                HasNoPermissionsToAdministerBuildPermissions = d
            };

            buildRights.BuildRightsIsSecure.ShouldBe(e);
            
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
    }
}