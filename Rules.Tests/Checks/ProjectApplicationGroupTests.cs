using System;
using System.Collections.Generic;
using SecurePipelineScan.Rules.Checks;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace Rules.Tests
{
    public class ProjectApplicationGroupTests
    {
        [Fact]
        public void EmptyGroupsShouldBeFalse()
        {
            ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(new List<ApplicationGroup>())
                .ShouldBeFalse();
        }

        [Fact]
        public void ContainingProductionEnvironmentOwnersShouldBeTrue()
        {
            ProjectApplicationGroup.ApplicationGroupContainsProductionEnvironmentOwner(new[]
                {new ApplicationGroup {FriendlyDisplayName = "Production Environment Owners"}}).ShouldBeTrue();
        }

        [Fact]
        public void ApplicationGroupContainingRabobankProjectAdministratorShouldBeTrue()
        {
            ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(new []
                {new ApplicationGroup {FriendlyDisplayName = "Rabobank Project Administrators"}}).ShouldBeTrue();
        }
        
        [Fact]
        public void ApplicationGroupNotContainingRabobankProjectAdministratorShouldBeTrue()
        {
            ProjectApplicationGroup.ProjectAdministratorsGroupOnlyContainsRabobankProjectAdministratorsGroup(new[]
                {new ApplicationGroup {FriendlyDisplayName = "No Rabo Project Admins"}}).ShouldBeFalse();
        }
    }
}