using System;
using System.Collections.Generic;
using System.Linq;
using Rules.Rules.Release;
using Shouldly;
using Xunit;

namespace Rules.Tests.Rules.Release
{
    public class LastModifiedByNotTheSameAsApprovedByTests
    {
        private readonly IReleaseRule rule = new LastModifiedByNotTheSameAsApprovedBy();
        
        [Fact]
        public void GivenReleaseModified_ApprovedByEqualsLastModifiedBy_ThenResultFalse()
        {
            var id = Guid.NewGuid();
            var release = NewRelease(id, id);

            rule.GetResult(release).ShouldBeFalse();
        }

        [Fact]
        public void GivenReleaseModified_ApprovedByNotEqualsLastModifiedBy_ThenResultTrue()
        {
            var release = NewRelease(Guid.NewGuid(), Guid.NewGuid());
            rule.GetResult(release).ShouldBeTrue();
        }

        private static Vsts.Response.Release NewRelease(Guid id, Guid modified)
        {
            return new Vsts.Response.Release
            {
                Environments = new List<Vsts.Response.Environment>
                {
                    new Vsts.Response.Environment
                    {
                        DeploySteps = new List<Vsts.Response.DeployStep>
                        {
                            new Vsts.Response.DeployStep
                            {
                                LastModifiedBy = new Vsts.Response.Identity
                                {
                                    Id = modified
                                }
                            }
                        },
                        PreDeployApprovals = new List<Vsts.Response.PreDeployApproval>
                        {
                            new Vsts.Response.PreDeployApproval
                            {
                                ApprovedBy = new Vsts.Response.Identity
                                {
                                    Id = id
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}