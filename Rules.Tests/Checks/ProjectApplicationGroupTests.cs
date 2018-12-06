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
            ProjectApplicationGroup.HasRequiredReviewerPolicy(new List<ApplicationGroup>()).ShouldBeFalse();
        }

        [Fact]
        public void ContainingProductionEnvironmentOwnersShouldBeTrue()
        {
            ProjectApplicationGroup.HasRequiredReviewerPolicy(new[] { new ApplicationGroup { FriendlyDisplayName = "Production Environment Owners" } }).ShouldBeTrue();
        }
    }
}
