using System;
using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.Rules.Release;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Tests.Rules.Release
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

        private static Response.Release NewRelease(Guid id, Guid modified)
        {
            return new Response.Release
            {
                Environments = new List<Response.Environment>
                {
                    new Response.Environment
                    {
                        DeploySteps = new List<Response.DeployStep>
                        {
                            new Response.DeployStep
                            {
                                LastModifiedBy = new Response.Identity
                                {
                                    Id = modified
                                }
                            }
                        },
                        PreDeployApprovals = new List<Response.PreDeployApproval>
                        {
                            new Response.PreDeployApproval
                            {
                                ApprovedBy = new Response.Identity
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