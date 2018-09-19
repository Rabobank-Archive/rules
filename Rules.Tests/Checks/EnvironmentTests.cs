using System;
using System.Linq;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Checks.Tests
{
    public class EnvironmentTest
    {
        [Fact]
        public void IsApprovedByIsFalseWhenNoApprovals()
        {
            var env = new Response.Environment
            {
            };

            var approved = env.IsApprovedBySomeoneElse();
            approved.ShouldBeFalse();
        }

        [Fact]
        public void IsApprovedIsTrueWhenIdIsDifferentFromRequestedFor()
        {
            var env = NewEnvironment(Guid.NewGuid(), Guid.NewGuid());
            var approved = env.IsApprovedBySomeoneElse();

            approved.ShouldBeTrue();
        }

        [Fact]
        public void IsApprovedIsFalseWhenIdIsSameAsRequestedFor()
        {
            var id = Guid.NewGuid();
            var env = NewEnvironment(id, id);
            var approved = env.IsApprovedBySomeoneElse();

            approved.ShouldBeFalse();
        }

        [Fact]
        public void GivenAutomaticApproved_WhenCheckForApprovalBySomeoneElse_ThenResultIsFalse()
        {
            var env = new Response.Environment {
                PreDeployApprovals = new [] {
                    new Response.PreDeployApproval {
                        ApprovalType = "preDeploy",
                        Status = "approved",
                        IsAutomated = true
                    }
                },
                DeploySteps = new [] {
                    new Response.DeployStep {
                        RequestedFor = new Response.Identity {
                            Id = Guid.NewGuid()
                        }
                    }
                }
            };

            env.IsApprovedBySomeoneElse().ShouldBeFalse();
        }

        private static Response.Environment NewEnvironment(Guid approvedBy, Guid requestedFor)
        {
            return new Response.Environment
            {
                PreDeployApprovals = new[] {
                new Response.PreDeployApproval {
                    ApprovedBy = new Response.Identity {
                        Id = approvedBy
                    }
                }
            },
                DeploySteps = new[] {
                new Response.DeployStep {
                    RequestedFor = new Response.Identity {
                        Id = requestedFor
                    }
                }
            }
            };
        }
    }
}