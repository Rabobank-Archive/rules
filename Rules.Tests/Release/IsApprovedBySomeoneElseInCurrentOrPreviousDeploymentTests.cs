using System;
using NSubstitute;
using SecurePipelineScan.Rules.Checks;
using Shouldly;
using Xunit;
using r = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release.Tests
{
    public class IsApprovedBySomeoneElseInCurrentOrPreviousDeploymentTests
    {
        [Fact]
        public void EmpyReleaseIsNotApproved()
        {
            var release = new r.Release {
            };

            var rule = new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment();
            rule.GetResult(release, "110").ShouldBeFalse();
        }

        [Fact]
        public void OneEnvironmentAndApproved()
        {
            var release = new r.Release {
                Environments = new[] {
                    new r.Environment {
                        Id = "110"
                    }
                }
            };

            var rule = new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment(_ => true);
            rule.GetResult(release, "110").ShouldBeTrue();
        }

        [Fact]
        public void WrongEnvironmentIsNotApproved()
        {
            var release = new r.Release {
                Environments = new[] {
                    new r.Environment {
                        Id = "110"
                    }
                }
            };

            var rule = new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment();
            rule.GetResult(release, "111").ShouldBeFalse();
        }


        [Fact]
        public void ProperEnvironemntButNotApproved()
        {
            var release = new r.Release {
                Environments = new[] {
                    new r.Environment {
                        Id = "110"
                    }
                }
            };

            var rule = new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment(_ => false);
            rule.GetResult(release, "110").ShouldBeFalse();
        }

        [Fact]
        public void CheckAllPreviousEnvironments()
        {
            var release = new r.Release {
                Environments = new[] {
                    new r.Environment {
                        Id = "110",
                        Name = "validation-this-one",
                        Conditions = new [] {
                            new r.Condition {
                                Result = true,
                                Name =  "first",
                                ConditionType =  "environmentState",
                                Value =  "4"
                            }
                        }
                    },
                    new r.Environment {
                        Name = "first"
                    }
                }
            };

            var approved = Substitute.For<Func<r.Environment, bool>>();
            approved.Invoke(Arg.Any<r.Environment>()).Returns(false);
            approved.Invoke(Arg.Is<r.Environment>(e => e.Name == "first")).Returns(true);


            var rule = new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment(approved);
            rule.GetResult(release, "110").ShouldBeTrue();
            approved.Received().Invoke(Arg.Is<r.Environment>(e => e.Name == "first"));
        }

        [Fact]
        public void UsingDefaultIsApprovedFunction()
        {
            new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment();
        }

        [Fact]
        public void UsingOverloadedTestConstructorThrowsWhenNoFunctionSpecified()
        {
            Assert.Throws<ArgumentNullException>(() => new IsApprovedBySomeoneElseInCurrentOrPreviousDeployment(null));
        }
    }
}