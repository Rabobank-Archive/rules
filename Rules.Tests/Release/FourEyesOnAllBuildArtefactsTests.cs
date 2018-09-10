using System;
using NSubstitute;
using SecurePipelineScan.Rules.Checks;
using Shouldly;
using Xunit;
using r = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release.Tests
{
    public class FourEyesOnAllBuildArtefactsTests
    {
        [Fact]
        public void EmpyReleaseIsNotApproved()
        {
            var release = new r.Release {
            };

            var rule = new FourEyesOnAllBuildArtefacts();
            rule.GetResult(release, "110").ShouldBeFalse();
        }

        [Fact]
        public void OneEnvironmentAndApproved()
        {
            var release = new r.Release {
                Environments = new[] {
                    new r.Environment {
                        Id = "110",
                        Name = "single"
                    }
                }
            };

            var rule = new FourEyesOnAllBuildArtefacts(_ => true);
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

            var rule = new FourEyesOnAllBuildArtefacts();
            rule.GetResult(release, "111").ShouldBeFalse();
        }


        [Fact]
        public void ProperEnvironemntButNotApproved()
        {
            var release = new r.Release {
                Environments = new[] {
                    new r.Environment {
                        Id = "110",
                        Name = "single"
                    }
                }
            };

            var rule = new FourEyesOnAllBuildArtefacts(_ => false);
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


            var rule = new FourEyesOnAllBuildArtefacts(approved);
            rule.GetResult(release, "110").ShouldBeTrue();
            approved.Received().Invoke(Arg.Is<r.Environment>(e => e.Name == "first"));
        }

        [Fact]
        public void UsingDefaultIsApprovedFunction()
        {
            new FourEyesOnAllBuildArtefacts();
        }

        [Fact]
        public void UsingOverloadedTestConstructorThrowsWhenNoFunctionSpecified()
        {
            Assert.Throws<ArgumentNullException>(() => new FourEyesOnAllBuildArtefacts(null));
        }
    }
}