using System;
using System.Collections.Generic;
using Rules.Rules.Release;
using Shouldly;
using Xunit;

namespace Rules.Tests.Rules.Release
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

        private static Vsts.Response.Release NewAutomatedRelease()
        {
            return new Vsts.Response.Release
            {
                Environments = new List<Vsts.Response.Environment> {
                    new Vsts.Response.Environment {
                        PreDeployApprovals = new List<Vsts.Response.PreDeployApproval> {
                            new Vsts.Response.PreDeployApproval {
                                IsAutomated = true
                            }
                        }
                    }
                }
            };
        }

        private static Vsts.Response.Release NewRelease(Guid requestFor, Guid approvedBy)
        {
            //Given
            return new Vsts.Response.Release
            {
                Environments = new List<Vsts.Response.Environment> {
                    new Vsts.Response.Environment {
                        DeploySteps = new List<Vsts.Response.DeployStep> {
                            new Vsts.Response.DeployStep {
                                RequestedFor = new Vsts.Response.Identity {
                                    Id = requestFor
                                }
                            }
                        },
                        PreDeployApprovals = new List<Vsts.Response.PreDeployApproval>{
                            new Vsts.Response.PreDeployApproval {
                                ApprovedBy = new Vsts.Response.Identity {
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