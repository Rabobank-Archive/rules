using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.Rules.Release;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using Rules.Tests.Release;

namespace SecurePipelineScan.Tests.Rules.Release
{
    public class LastModifiedByNotTheSameAsApprovedByTests
    {
        private readonly IReleaseRule rule = new LastModifiedByNotTheSameAsApprovedBy();
        
        [Fact]
        public void GivenReleaseModified_ApprovedByEqualsLastModifiedBy_ThenResultFalse()
        {
            var id = Guid.NewGuid();
            var release = ReleaseHelper.NewRelease(id, id);

            rule.GetResult(release).ShouldBeFalse();
        }

        [Fact]
        public void GivenReleaseModified_ApprovedByNotEqualsLastModifiedBy_ThenResultTrue()
        {
            var release = ReleaseHelper.NewRelease(Guid.NewGuid(), Guid.NewGuid());
            rule.GetResult(release).ShouldBeTrue();
        }

    }
}