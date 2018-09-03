using System;
using System.Collections.Generic;
using lib.Rules.Release;
using Shouldly;
using Xunit;

namespace lib.tests.Rules.Release
{
    public class ApprovedForNotTheSameAsRequestedForTests
    {
        private readonly IReleaseRule rule = new ApprovedForNotTheSameAsRequestedFor();

        [Fact]
        public void GivenPreDeployApprovalIsNotAutomated_WhenApprovedByIsSameAsRequestedFor_ThenResultIsFalse()
        {
            var id = Guid.NewGuid();
            var release = NewRelease(id, id);

            //When
            var result = rule.GetResult(release);

            //Then
            result.ShouldBe(false);
        }

        [Fact]
        public void GivenPreDeployApprovalIsNotAutomated_WhenApprovedByIsDifferentFromRequestedFor_ThenResultIsTrue()
        {
            var release = NewRelease(Guid.NewGuid(), Guid.NewGuid());

            //When
            var result = rule.GetResult(release);

            //Then
            result.ShouldBe(true);
        }

        [Fact]
        public void GivenPreDeployApprovalIsAutomated_WhenScanningRequestedForAndApprovedBy_ThenResultIsTrue()
        {
            var release = NewAutomatedRelease();

            //When
            var result = rule.GetResult(release);

            //Then
            result.ShouldBe(true);
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