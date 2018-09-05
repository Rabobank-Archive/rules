using System.Collections.Generic;
using System.Linq;
using Rules.Rules.Release;
using Shouldly;
using Xunit;

namespace Rules.Tests.Rules.Release
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

        private static Vsts.Response.Release NewRelease()
        {
            return new Vsts.Response.Release
            {
                Environments = new List<Vsts.Response.Environment> {
                    new Vsts.Response.Environment {
                        PreDeployApprovals = new List<Vsts.Response.PreDeployApproval> {
                            new Vsts.Response.PreDeployApproval {
                                IsAutomated = false
                            }
                        }
                    }
                }
            };
        }

        private static Vsts.Response.Release NewAutomatedRelease()
        {
            return new Vsts.Response.Release
            {
                Environments = new List<Vsts.Response.Environment>
                {
                    new Vsts.Response.Environment {
                        PreDeployApprovals = new List<Vsts.Response.PreDeployApproval>
                        {
                            new Vsts.Response.PreDeployApproval {
                                IsAutomated = true
                            }
                        }
                    }
                }
            };
        }
    }
}