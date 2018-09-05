using System;
using System.Collections.Generic;
using System.Linq;
using lib.Rules.Release;
using Shouldly;
using Xunit;

namespace lib.tests.Rules.Release
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

        private static vsts.Response.Release NewRelease(Guid id, Guid modified)
        {
            return new vsts.Response.Release
            {
                Environments = new List<vsts.Response.Environment>
                {
                    new vsts.Response.Environment
                    {
                        DeploySteps = new List<vsts.Response.DeployStep>
                        {
                            new vsts.Response.DeployStep
                            {
                                LastModifiedBy = new vsts.Response.Identity
                                {
                                    Id = modified
                                }
                            }
                        },
                        PreDeployApprovals = new List<vsts.Response.PreDeployApproval>
                        {
                            new vsts.Response.PreDeployApproval
                            {
                                ApprovedBy = new vsts.Response.Identity
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