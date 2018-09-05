using System.Collections.Generic;
using System.Linq;
using SecurePipelineScan.Rules.Release;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Tests.Rules.Release
{
    public class ManualApproverRequiredTests
    {
        private readonly IReleaseRule rule = new ManualApproverRequired();

        [Fact]
        public void AllEnvironments_AutomaticApproval_ResultsFalse()
        {
            var release = NewAutomatedRelease();
            rule.GetResult(release).ShouldBeFalse();
        }

        [Fact]
        public void AtLeastOneManualApproval_ResultIsTrue()
        {
            var release = NewRelease();
            rule.GetResult(release).ShouldBeTrue();
        }

        private static Response.Release NewRelease()
        {
            return new Response.Release
            {
                Environments = new List<Response.Environment> {
                    new Response.Environment {
                        PreDeployApprovals = new List<Response.PreDeployApproval> {
                            new Response.PreDeployApproval {
                                IsAutomated = false
                            }
                        }
                    }
                }
            };
        }

        private static Response.Release NewAutomatedRelease()
        {
            return new Response.Release
            {
                Environments = new List<Response.Environment>
                {
                    new Response.Environment {
                        PreDeployApprovals = new List<Response.PreDeployApproval>
                        {
                            new Response.PreDeployApproval {
                                IsAutomated = true
                            }
                        }
                    }
                }
            };
        }
    }
}