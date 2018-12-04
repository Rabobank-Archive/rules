using System;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.Rules.Tests;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release.Tests
{
    public class FourEyesOnAllBuildArtefactsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;

        public FourEyesOnAllBuildArtefactsTests(ITestOutputHelper output, TestConfig config)
        {
            this.config = config;
        }

        [Fact]
        [Trait("category", "integration")]
        public void IntegrationTest714()
        {
            var client = new VstsRestClient(config.Organization, config.Token);
            var rule = new FourEyesOnAllBuildArtefacts();

            var release = client.Execute(new VstsRestRequest<Response.Release>("https://somecompany.vsrm.visualstudio.com/f64ffdfa-0c4e-40d9-980d-bb8479366fc5/_apis/Release/releases/741", Method.GET)).ThrowOnError();
            rule.GetResult(release.Data, 1916).ShouldBeTrue();
        }

        [Fact]
        public void EmptyReleaseIsNotApproved()
        {
            var release = new Response.Release {
            };

            var rule = new FourEyesOnAllBuildArtefacts();
            rule.GetResult(release, 110).ShouldBeFalse();
        }

        [Fact]
        public void OneEnvironmentAndApproved()
        {
            var release = new Response.Release {
                Environments = new[] {
                    new Response.Environment {
                        Id = 110,
                        Name = "single"
                    }
                }
            };

            var rule = new FourEyesOnAllBuildArtefacts(_ => true);
            rule.GetResult(release, 110).ShouldBeTrue();
        }

        [Fact]
        public void WrongEnvironmentIsNotApproved()
        {
            var release = new Response.Release {
                Environments = new[] {
                    new Response.Environment {
                        Id = 110
                    }
                }
            };

            var rule = new FourEyesOnAllBuildArtefacts();
            rule.GetResult(release, 111).ShouldBeFalse();
        }


        [Fact]
        public void ProperEnvironemntButNotApproved()
        {
            var release = new Response.Release {
                Environments = new[] {
                    new Response.Environment {
                        Id = 110,
                        Name = "single"
                    }
                }
            };

            var rule = new FourEyesOnAllBuildArtefacts(_ => false);
            rule.GetResult(release, 110).ShouldBeFalse();
        }

        [Fact]
        public void GivenCurrentEnvironmentIsNotApproved_WhenConditionsAreNotPreviousEnvironment_ThenResultIsFalse()
        {
            var release = new Response.Release {
                Environments = new [] {
                    new Response.Environment {
                        Id = 109,
                        Name = "doesn't-really-matter",
                        Conditions = new [] {
                            new Response.Condition {
                                Result = true,
                                Name = "ReleaseStarted",
                                ConditionType = "event",
                                Value = ""
                            }
                        }
                    }
                }
            };

            var rule = new FourEyesOnAllBuildArtefacts(_ => false);
            rule.GetResult(release, 109).ShouldBeFalse();
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