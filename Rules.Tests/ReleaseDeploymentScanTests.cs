using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                report.HasApprovalOptions.ShouldBe(true);
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
                report.HasApprovalOptions.ShouldBe(false);
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
                    .ShouldBe(false);
            }
            
            [Fact]
            public void NoBranchFilterForAllArtifacts()
            {
                var input = ReadInput("Completed", "Approved.json");
                    
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                
                var report = scan.Completed(input);
                report.HasBranchFilterForAllArtifacts.ShouldBe(false);
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
                report.HasBranchFilterForAllArtifacts.ShouldBe(true);
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
                }.ToExpectedObject(ctx =>
                {
                    ctx.Ignore(x => x.HasApprovalOptions);
                    ctx.Ignore(x => x.UsesProductionEndpoints);
                    ctx.Ignore(x => x.UsesManagedAgentsOnly);
                    ctx.Ignore(x => x.RelatedToSm9Change);
                    ctx.Ignore(x => x.AllArtifactsAreFromBuild);
                    ctx.Ignore(x => x.HasBranchFilterForAllArtifacts);
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
                
                report.UsesProductionEndpoints.ShouldBe(false);
            }

            [Fact]
            public void HowToHandleDefaults()
            {
                var expected = new ReleaseDeploymentCompletedReport
                {
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

                var client = Substitute.For<IVstsRestClient>();
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
                _fixture
                    .Customize<Response.DeployPhaseSnapshot>(context =>
                        context.With(x => x.PhaseType, "agentBasedDeployment"));
                
                var input = ReadInput("Completed", "Approved.json");

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .Get(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                var deployPhaseSnapshot = _fixture.Create<Response.DeployPhaseSnapshot>();
                deployPhaseSnapshot.PhaseType = "agentBasedDeployment";
                var environment = _fixture.Create<Response.Environment>();
                environment.DeployPhasesSnapshot = new[] {deployPhaseSnapshot}; 
                rest
                    .Get(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(environment);

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest);
                var report = scan.Completed(input);

                // Assert
                Assert.True(report.UsesManagedAgentsOnly);
                rest.Received()
                    .Get(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                        r.Uri.Contains(deployPhaseSnapshot.DeploymentInput.QueueId.ToString())));
            }

            [Fact]
            public void ForTheRestCallToTheAgentQueueOnlyQueueIdsFromTheInputAreUsed()
            {
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, 115));
                _fixture.Customize<Response.DeployPhaseSnapshot>(context =>
                    context.With(x => x.PhaseType, "agentBasedDeployment"));

                var environmentFixture = _fixture.Create<Response.Environment>();
                
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
                    .Returns(environmentFixture);

                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest);
                scan.Completed(input);

                rest.Received(environmentFixture.DeployPhasesSnapshot.Count())
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>());

                foreach (var phase in environmentFixture.DeployPhasesSnapshot)
                {
                    rest.Received().Get(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                        r.Uri.Contains(phase.DeploymentInput.QueueId.ToString())));
                }

            }

            [Fact]
            public void DontCheckTasManagedAgentsForNonAgentBasedDeployments()
            {
                // Arrange
                _fixture
                    .Customize<Response.DeployPhaseSnapshot>(context =>
                        context.With(x => x.PhaseType, "machineGroupBasedDeployment"));
                
                var input = ReadInput("Completed", "Approved.json");
    
                var rest = Substitute.For<IVstsRestClient>();
    
                rest
                    .Get(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());
                    
    
                // Act
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), rest);
                var report = scan.Completed(input);
    
                // Assert
                rest.DidNotReceive()
                    .Get(Arg.Any<IVstsRequest<Response.AgentQueue>>());
                Assert.Null(report.UsesManagedAgentsOnly);
            }

            [Fact]
            public void IfQueueIdResultsInUnmanagedPoolIdThenFalse()
            {
                // This is most definetely not an managed pool id.
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, 543));
                _fixture.Customize<Response.DeployPhaseSnapshot>(context =>
                    context.With(x => x.PhaseType, "agentBasedDeployment"));
                
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
                var artifacts = new []
                {
                    new Response.ArtifactReference() {Alias = "Rianne", Type = "Build"}
                };

                _fixture
                    .Customize<Response.Release>(context => context.With(x => x.Artifacts, artifacts));

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
                Assert.True(report.AllArtifactsAreFromBuild);
            }

            [Fact]
            public void ShouldReturnFalseIfArtifactsAreNotFromTypeBuild()
            {
                // Arrange
                var artifacts = new []
                {
                    new Response.ArtifactReference() {Alias = "Rianne2", Type = "repository"}
                };

                _fixture
                    .Customize<Response.Release>(context => context.With(x => x.Artifacts, artifacts));

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
                Assert.False(report.AllArtifactsAreFromBuild);
            }

            [Fact]
            public void ShouldReturnFalseIfArtifactsCountEqualsZero()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved2.json");

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
                Assert.False(report.AllArtifactsAreFromBuild);
            }

            [Fact]
            public void NoInformationIfReleaseInformationIsUnavailable()
            {
                var expected = new ReleaseDeploymentCompletedReport().ToExpectedObject(ctx =>
                {
                    // Ignore static members that are directly deduced from the message
                    ctx.Ignore(x => x.CreatedDate);
                    ctx.Ignore(x => x.Project);
                    ctx.Ignore(x => x.Pipeline);
                    ctx.Ignore(x => x.Release);
                    ctx.Ignore(x => x.Environment);
                    ctx.Ignore(x => x.ReleaseId);
                    ctx.Ignore(x => x.UsesManagedAgentsOnly);
                });

                var input = ReadInput("Completed", "Approved2.json");
                var client = Substitute.For<IVstsRestClient>();
                client.Get(Arg.Any<IVstsRequest<Response.AgentQueue>>()).Returns(_fixture.Create<Response.AgentQueue>());
                
                var scan = new ReleaseDeploymentScan(Substitute.For<IServiceEndpointValidator>(), client);
                var report = scan.Completed(input);
                
                expected.ShouldEqual(report);
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
                                    phaseType = "agentBasedDeployment",
                                    deploymentInput = new
                                    {
                                        queueId = 1234553
                                    }
                                },
                                new
                                {
                                    phaseType = "agentBasedDeployment",
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