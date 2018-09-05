using System.Collections.Generic;
using System.Linq;
using lib.Rules.Release;
using Shouldly;
using Xunit;

namespace lib.tests.Rules.Release
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

        private static vsts.Response.Release NewRelease()
        {
            return new vsts.Response.Release
            {
                Environments = new List<vsts.Response.Environment> {
                    new vsts.Response.Environment {
                        PreDeployApprovals = new List<vsts.Response.PreDeployApproval> {
                            new vsts.Response.PreDeployApproval {
                                IsAutomated = false
                            }
                        }
                    }
                }
            };
        }

        private static vsts.Response.Release NewAutomatedRelease()
        {
            return new vsts.Response.Release
            {
                Environments = new List<vsts.Response.Environment>
                {
                    new vsts.Response.Environment {
                        PreDeployApprovals = new List<vsts.Response.PreDeployApproval>
                        {
                            new vsts.Response.PreDeployApproval {
                                IsAutomated = true
                            }
                        }
                    }
                }
            };
        }
    }
}