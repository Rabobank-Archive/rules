using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Checks
{
    public class ProjectApplicationGroupTests
    {
        [Fact]
        public void EmptyGroupsShouldBeFalse()
        {
            ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(new List<VstsService.Response.ApplicationGroup>())
                .ShouldBeFalse();
        }

        [Fact]
        public void ContainingProductionEnvironmentOwnersShouldBeTrue()
        {
            ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(new[]
                {new VstsService.Response.ApplicationGroup {FriendlyDisplayName = "Production Environment Owners"}}).ShouldBeTrue();
        }

        [Fact]
        public void ApplicationGroupContainingRabobankProjectAdministratorShouldBeTrue()
        {
            ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(new[]
                {new VstsService.Response.ApplicationGroup {FriendlyDisplayName = "Rabobank Project Administrators"}}).ShouldBeTrue();
        }

        [Fact]
        public void ApplicationGroupNotContainingRabobankProjectAdministratorShouldBeFalse()
        {
            ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(new[]
                {new VstsService.Response.ApplicationGroup {FriendlyDisplayName = "No Rabo Project Admins"}}).ShouldBeFalse();
        }

        [Fact]
        public void ApplicationGroupsWithSizeMoreThenOneShouldReturnFalse()
        {
            ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(new[]
                    {new VstsService.Response.ApplicationGroup {FriendlyDisplayName = "Rabobank Project Administrators"}, new VstsService.Response.ApplicationGroup {FriendlyDisplayName = "Yoyoyo"}})
                .ShouldBeFalse();
        }
    }
}