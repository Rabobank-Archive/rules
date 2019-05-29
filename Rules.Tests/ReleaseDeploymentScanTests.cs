using System;
using System.Collections.Generic;
using System.IO;
using AutoFixture;
using ExpectedObjects;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Events;
using SecurePipelineScan.Rules.Reports;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Tests
{
    public class ReleaseDeploymentScanTests
    {
        public class Completed
        {
            private readonly IFixture _fixture = new Fixture();
            
            [Fact]
            public void ApprovalSettingsCorrect()
            {
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.ApprovalOptions>(x => x
                    .With(a => a.ReleaseCreatorCanBeApprover, false));

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                
                var report = scan.Completed(input);
                report.HasApprovalOptions.ShouldBeTrue();
            }

            [Fact]
            public void OnlyAutomatedApprovals()
            {
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.ApprovalOptions>(x => x
                    .With(a => a.ReleaseCreatorCanBeApprover, false));

                _fixture.Customize<Response.Approval>(x => x
                    .With(a => a.IsAutomated, true));
                    
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                
                var report = scan.Completed(input);
                report.HasApprovalOptions.ShouldBeFalse();
            }

            [Fact]
            public void RequestedForCanBeApprover()
            {
                var input = ReadInput("Completed", "NotApproved.json");
                _fixture
                    .Customize<Response.ApprovalOptions>(x => x.With(a => a.ReleaseCreatorCanBeApprover, true));

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);

                var report = scan.Completed(input);
                report
                    .HasApprovalOptions
                    .ShouldBeFalse();
            }
            
            [Fact]
            public void NoBranchFilterForAllArtifacts()
            {
                var input = ReadInput("Completed", "Approved.json");
                    
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                
                var report = scan.Completed(input);
                report.HasBranchFilterForAllArtifacts.ShouldBeFalse();
            }
            
            [Fact]
            public void BranchFilterForAllArtifacts()
            {
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(x => x
                    .With(a => a.Artifacts, new []{new Response.ArtifactReference { Alias = "some-build-artifact"}}));

                _fixture.Customize<Response.Environment>(x =>
                    x.With(v => v.Conditions, new[]
                    {
                        new Response.Condition
                        {
                            ConditionType = "artifact",
                            Name = "some-build-artifact"
                        }
                    }));
                    
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                
                var report = scan.Completed(input);
                report.HasBranchFilterForAllArtifacts.ShouldBeTrue();
            }


            [Fact]
            public void ReportInformation()
            {
                var expected = new ReleaseDeploymentCompletedReport
                {
                    Project = "proeftuin",
                    Pipeline = "CheckApproval",
                    Release = "Release-1",
                    Environment = "Stage 1",
                    ReleaseId = "1",
                    CreatedDate = DateTime.Parse("2019-01-11T13:34:58.0366887"),
                    AllArtifactsAreFromBuild = false
                }.ToExpectedObject(ctx =>
                {
                    ctx.Ignore(x => x.HasApprovalOptions);
                    ctx.Ignore(x => x.UsesProductionEndpoints);
                    ctx.Ignore(x => x.UsesManagedAgentsOnly);
                    ctx.Ignore(x => x.RelatedToSm9Change);
                });

                var input = ReadInput("Completed", "Approved.json");
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);

                var report = scan.Completed(input);
                expected.ShouldEqual(report);
            }

            [Fact]
            public void UsesServiceEndpointValidatorToScan()
            {
                var input = ReadInput("Completed", "NotApproved.json");
                _fixture.Customize<Response.WorkflowTask>(x => x.With(a => a.Inputs, new Dictionary<string, string>
                {
                    ["some-input"] = Guid.NewGuid().ToString()
                }));
                
                var endpoints = Substitute.For<IServiceEndpointValidator>();
                endpoints
                    .IsProduction(Arg.Any<string>(), Arg.Any<Guid>())
                    .Returns(true);
                
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(endpoints, client);
                scan.Completed(input);
                
                endpoints
                    .Received()
                    .IsProduction("TAS", Arg.Any<Guid>());
            }

            [Fact]
            public void IgnoresTasks()
            {
                _fixture
                    .Customize<Response.WorkflowTask>(
                        x => x
                            .With(y => y.Inputs, new Dictionary<string, string> {["connection"] = Guid.NewGuid().ToString()})
                            .With(y => y.TaskId, new Guid("dd84dea2-33b4-4745-a2e2-d88803403c1b")));
                
                var input = JObject.FromObject(new
                {
                    createdDate = "2019-01-11T13:34:58.0366887Z"
                });
                
                var endpoints = Substitute.For<IServiceEndpointValidator>();
                endpoints
                    .IsProduction(Arg.Any<string>(), Arg.Any<Guid>())
                    .Returns(true);
                
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(endpoints, client);
                var report = scan.Completed(input);
                
                report.UsesProductionEndpoints.ShouldBeFalse();
            }

            [Fact]
            public void HowToHandleDefaults()
            {
                var expected = new ReleaseDeploymentCompletedReport
                {
                    UsesManagedAgentsOnly = true,
                    HasApprovalOptions = true,
                    AllArtifactsAreFromBuild = false,
                    RelatedToSm9Change = false
                    // All default null values and false for booleans is fine
                }.ToExpectedObject(ctx => ctx.Member(x => x.CreatedDate).UsesComparison(Expect.NotDefault<DateTime>()));
                
                var input = JObject.FromObject(new
                {
                    createdDate = "2019-01-11T13:34:58.0366887Z",
                    resource = new
                    {
                        environment = new
                        {
                            deployPhasesSnapshots = new object[]
                            {
                            }
                        }
                    }
                });
                
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);

                var report = scan.Completed(input);
                expected.ShouldEqual(report);
            }

            [Theory]
            [InlineData(114)]
            [InlineData(115)]
            [InlineData(116)]
            [InlineData(117)]
            [InlineData(119)]
            [InlineData(120)]
            [InlineData(121)]
            [InlineData(122)]
            
            public void AgentIsTasManagedAgent(int poolId)
            {
                // Arrange
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, poolId));
                var input = ReadInput("Completed", "Approved.json");

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .Get(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());
                
                rest
                    .Get(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest);
                var report = scan.Completed(input);

                // Assert
                Assert.True(report.UsesManagedAgentsOnly);
                rest.Received()
                    .Get(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                        r.Uri == "/proeftuin/_apis/distributedtask/queues/1665"));
            }

            [Fact]
            public void ForTheRestCallToTheAgentQueueOnlyQueueIdsFromTheInputAreUsed()
            {
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, 115));
                
                var input = ReleaseCompletedInput();

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .Get(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());
                
                rest
                    .Get(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest);
                scan.Completed(input);

                // Assert
                rest.Received().Get(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                    r.Uri == "/proeftuin/_apis/distributedtask/queues/653456"));

                rest.Received().Get(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                    r.Uri == "/proeftuin/_apis/distributedtask/queues/1234553"));
            }

            [Fact]
            public void IfQueueIdResultsInUnmanagedPoolIdThenFalse()
            {
                // This is most definetely not an managed pool id.
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, 543));

                var input = ReleaseCompletedInput();

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .Get(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());
                rest
                    .Get(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest);
                var report = scan.Completed(input);

                // Assert
                Assert.False(report.UsesManagedAgentsOnly);
            }

            [Fact]
            public void AllArtifactAreFromBuild()
            {
                // Arrange
                var artifacts = new List<Response.ArtifactReference>();
                artifacts.Add(new Response.ArtifactReference() { Alias = "Rianne", Type = "Build" });

                _fixture
                    .Customize<Response.Release>(context => context.With(x => x.Artifacts, artifacts));

                var input = ReadInput("Completed", "Approved.json");

                var rest = new Mock<IVstsRestClient>(MockBehavior.Strict);
                rest
                    .Setup(x => x.Get(It.Is<IVstsRequest<Response.AgentQueue>>(r => r.Uri == "/proeftuin/_apis/distributedtask/queues/1665")))
                    .Returns(_fixture.Create<Response.AgentQueue>())
                    .Verifiable();

                rest
                    .Setup(x => x.Get(It.IsAny<IVstsRequest<Response.Release>>()))
                    .Returns(_fixture.Create<Response.Release>());
                rest
                    .Setup(x => x.Get(It.IsAny<IVstsRequest<Response.Environment>>()))
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest.Object);
                var report = scan.Completed(input);

                // Assert
                Assert.True(report.AllArtifactsAreFromBuild);
                rest.Verify();
            }

            [Fact]
            public void ShoudlReturnFalseIfArtifactsAreNotFromTypeBuild()
            {
                // Arrange
                var artifacts = new List<Response.ArtifactReference>();
                artifacts.Add(new Response.ArtifactReference() { Alias = "Rianne2", Type = "repository" });

                _fixture
                    .Customize<Response.Release>(context => context.With(x => x.Artifacts, artifacts));

                var input = ReadInput("Completed", "Approved.json");

                var rest = new Mock<IVstsRestClient>(MockBehavior.Strict);
                rest
                    .Setup(x => x.Get(It.Is<IVstsRequest<Response.AgentQueue>>(r => r.Uri == "/proeftuin/_apis/distributedtask/queues/1665")))
                    .Returns(_fixture.Create<Response.AgentQueue>())
                    .Verifiable();

                rest
                    .Setup(x => x.Get(It.IsAny<IVstsRequest<Response.Release>>()))
                    .Returns(_fixture.Create<Response.Release>());
                rest
                    .Setup(x => x.Get(It.IsAny<IVstsRequest<Response.Environment>>()))
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest.Object);
                var report = scan.Completed(input);

                // Assert
                Assert.False(report.AllArtifactsAreFromBuild);
                rest.Verify();
            }

            [Fact]
            public void ShoudlReturnFalseIfArtifactsCountEqualsZero()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved2.json");

                var rest = new Mock<IVstsRestClient>(MockBehavior.Strict);
                rest
                    .Setup(x => x.Get(It.Is<IVstsRequest<Response.AgentQueue>>(r => r.Uri == "/proeftuin/_apis/distributedtask/queues/1665")))
                    .Returns(_fixture.Create<Response.AgentQueue>())
                    .Verifiable();

                rest
                    .Setup(x => x.Get(It.IsAny<IVstsRequest<Response.Release>>()))
                    .Returns(_fixture.Create<Response.Release>());
                rest
                    .Setup(x => x.Get(It.IsAny<IVstsRequest<Response.Environment>>()))
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest.Object);
                var report = scan.Completed(input);

                // Assert
                Assert.False(report.AllArtifactsAreFromBuild);
                rest.Verify();
            }

            private static JObject ReleaseCompletedInput()
            {
                // Arrange
                return JObject.FromObject(new
                {
                    createdDate = "2019-01-11T13:34:58.0366887Z",
                    resource = new
                    {
                        environment = new
                        {
                            deployPhasesSnapshot = new[]
                            {
                                new
                                {
                                    deploymentInput = new
                                    {
                                        queueId = 1234553
                                    }
                                },
                                new
                                {
                                    deploymentInput = new
                                    {
                                        queueId = 653456
                                    }
                                }
                            }
                        },
                        project = new
                        {
                            name = "proeftuin"
                        }
                    }
                });
            }

            [Fact]
            public void ShouldReturnTrueForReleaseHasSm9ChangeId()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(
                    x => x.With(
                        a => a.Tags, new[] {"SM9ChangeId 12345", "Random tag"}));
                
                // Act
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                var report = scan.Completed(input);
                
                // Assert
                Assert.True(report.RelatedToSm9Change);
            }

            [Fact]
            public void EvaluateShouldReturnFalseForReleaseHasNoSm9ChangeId()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(
                    x => x.With(
                        a => a.Tags, new[] {"12345", "Random tag"}));
                
                // Act
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                var report = scan.Completed(input);
                
                // Assert
                Assert.False(report.RelatedToSm9Change);
            }
        }

                
        private static JObject ReadInput(string eventType, string file)
        {
            var dir = Path.Join("Assets", "ReleaseDeployment", eventType);
            return JObject.Parse(File.ReadAllText(Path.Join(dir, file)));
        }

    }
}