using System;
using System.Collections.Generic;
using SecurePipelineScan.Rules.Release;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Tests.Rules.Release
{
    public class ApprovedByNotTheSameAsRequestedForTests
    {
        private readonly IReleaseRule rule = new ApprovedByNotTheSameAsRequestedFor();

        [Fact]
        public void GivenPreDeployApprovalIsNotAutomated_WhenApprovedByIsSameAsRequestedFor_ThenResultIsFalse()
        {
            var id = Guid.NewGuid();
            var release = NewRelease(id, id);

            rule.GetResult(release).ShouldBe(false);
        }

        [Fact]
        public void GivenPreDeployApprovalIsNotAutomated_WhenApprovedByIsDifferentFromRequestedFor_ThenResultIsTrue()
        {
            var release = NewRelease(Guid.NewGuid(), Guid.NewGuid());
            rule.GetResult(release).ShouldBe(true);
        }

        [Fact]
        public void GivenPreDeployApprovalIsAutomated_WhenScanningRequestedForAndApprovedBy_ThenResultIsTrue()
        {
            var release = NewAutomatedRelease();
            rule.GetResult(release).ShouldBe(true);
        }

        [Fact]
        public void WhenNoApprovers_ResultIsTrue()
        {
            var release = new Response.Release
            {
                Environments = new List<Response.Environment>
                {
                    new Response.Environment {}
                }
            };

            rule.GetResult(release).ShouldBeTrue();
        }

        private static Response.Release NewAutomatedRelease()
        {
            return new Response.Release
            {
                Environments = new List<Response.Environment> {
                    new Response.Environment {
                        PreDeployApprovals = new List<Response.PreDeployApproval> {
                            new Response.PreDeployApproval {
                                IsAutomated = true
                            }
                        }
                    }
                }
            };
        }

        private static Response.Release NewRelease(Guid requestFor, Guid approvedBy)
        {
            //Given
            return new Response.Release
            {
                Environments = new List<Response.Environment> {
                    new Response.Environment {
                        DeploySteps = new List<Response.DeployStep> {
                            new Response.DeployStep {
                                RequestedFor = new Response.Identity {
                                    Id = requestFor
                                }
                            }
                        },
                        PreDeployApprovals = new List<Response.PreDeployApproval>{
                            new Response.PreDeployApproval {
                                ApprovedBy = new Response.Identity {
                                    Id = approvedBy
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}