using System;
using System.Collections.Generic;
using lib.Rules.Release;
using Shouldly;
using Xunit;

namespace lib.tests.Rules.Release
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

        private static vsts.Response.Release NewAutomatedRelease()
        {
            return new vsts.Response.Release
            {
                Environments = new List<vsts.Response.Environment> {
                    new vsts.Response.Environment {
                        PreDeployApprovals = new List<vsts.Response.PreDeployApproval> {
                            new vsts.Response.PreDeployApproval {
                                IsAutomated = true
                            }
                        }
                    }
                }
            };
        }

        private static vsts.Response.Release NewRelease(Guid requestFor, Guid approvedBy)
        {
            //Given
            return new vsts.Response.Release
            {
                Environments = new List<vsts.Response.Environment> {
                    new vsts.Response.Environment {
                        DeploySteps = new List<vsts.Response.DeployStep> {
                            new vsts.Response.DeployStep {
                                RequestedFor = new vsts.Response.Identity {
                                    Id = requestFor
                                }
                            }
                        },
                        PreDeployApprovals = new List<vsts.Response.PreDeployApproval>{
                            new vsts.Response.PreDeployApproval {
                                ApprovedBy = new vsts.Response.Identity {
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