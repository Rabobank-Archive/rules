using System;
using System.IO;
using System.Linq;
using AutoFixture;
using ExpectedObjects;
using Newtonsoft.Json.Linq;
using NSubstitute;
using SecurePipelineScan.Rules.Events;
using SecurePipelineScan.Rules.Reports;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Events
{
    public static class ReleaseDeploymentScanTests
    {
        public class Completed
        {
            private readonly IFixture _fixture = new Fixture();

            [Fact]
            public async Task ApprovalSettingsCorrect()
            {
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.ApprovalOptions>(x => x
                    .With(a => a.ReleaseCreatorCanBeApprover, false));

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                report.HasApprovalOptions.ShouldBe(true);
            }

            [Fact]
            public async Task OnlyAutomatedApprovals()
            {
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.ApprovalOptions>(x => x
                    .With(a => a.ReleaseCreatorCanBeApprover, false));

                _fixture.Customize<Response.Approval>(x => x
                    .With(a => a.IsAutomated, true));

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                report.HasApprovalOptions.ShouldBe(false);
            }

            [Fact]
            public async Task RequestedForCanBeApprover()
            {
                var input = ReadInput("Completed", "NotApproved.json");
                _fixture
                    .Customize<Response.ApprovalOptions>(x => x.With(a => a.ReleaseCreatorCanBeApprover, true));

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                report
                    .HasApprovalOptions
                    .ShouldBe(false);
            }

            [Fact]
            public async Task NoPreApprovalsSnapshotApprovalOptionsShouldFail()
            {
                var input = ReadInput("Completed", "NotApproved.json");
                _fixture
                    .Customize<Response.PreApprovalSnapshot>(x => x.With(a => a.ApprovalOptions, (Response.ApprovalOptions)null));

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                report
                    .HasApprovalOptions
                    .ShouldBe(false);
            }

            [Fact]
            public async Task NoBranchFilterForAllArtifacts()
            {
                var input = ReadInput("Completed", "Approved.json");

                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                report.HasBranchFilterForAllArtifacts.ShouldBe(false);
            }

            [Fact]
            public async Task BranchFilterForAllArtifacts()
            {
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(x => x
                    .With(a => a.Artifacts, new[] { new Response.ArtifactReference { Alias = "some-build-artifact" } }));

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
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                report.HasBranchFilterForAllArtifacts.ShouldBe(true);
            }


            [Fact]
            public async Task ReportInformation()
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
                    ctx.Ignore(x => x.UsesManagedAgentsOnly);
                    ctx.Ignore(x => x.AllArtifactsAreFromBuild);
                    ctx.Ignore(x => x.HasBranchFilterForAllArtifacts);
                });

                var input = ReadInput("Completed", "Approved.json");
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
                expected.ShouldEqual(report);
            }

            [Fact]
            public async Task HowToHandleDefaults()
            {
                var expected = new ReleaseDeploymentCompletedReport().ToExpectedObject(ctx =>
                    ctx.Member(x => x.CreatedDate).UsesComparison(Expect.NotDefault<DateTime>()));

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
                var scan = new ReleaseDeploymentScan(client);

                var report = await scan.GetCompletedReportAsync(input);
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
            public async Task AgentIsTasManagedAgent(int poolId)
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
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                var deployPhaseSnapshot = _fixture.Create<Response.DeployPhaseSnapshot>();
                deployPhaseSnapshot.PhaseType = "agentBasedDeployment";
                var environment = _fixture.Create<Response.Environment>();
                environment.DeployPhasesSnapshot = new[] { deployPhaseSnapshot };
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(environment);

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.True(report.UsesManagedAgentsOnly);
                await rest.Received()
                    .GetAsync(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                        r.Resource.Contains(deployPhaseSnapshot.DeploymentInput.QueueId.ToString())));
            }

            [Fact]
            public async Task ReturnFalseWhenAgentQueueDoesNotExist()
            {
                // Arrange
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, 114));
                _fixture
                    .Customize<Response.DeployPhaseSnapshot>(context =>
                        context.With(x => x.PhaseType, "agentBasedDeployment"));

                var input = ReadInput("Completed", "Approved.json");

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns((Response.AgentQueue)null);

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                var deployPhaseSnapshot = _fixture.Create<Response.DeployPhaseSnapshot>();
                deployPhaseSnapshot.PhaseType = "agentBasedDeployment";
                var environment = _fixture.Create<Response.Environment>();
                environment.DeployPhasesSnapshot = new[] { deployPhaseSnapshot };
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(environment);

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.False(report.UsesManagedAgentsOnly);
                await rest.Received()
                    .GetAsync(Arg.Is<IVstsRequest<Response.AgentQueue>>(r =>
                        r.Resource.Contains(deployPhaseSnapshot.DeploymentInput.QueueId.ToString())));
            }

            [Fact]
            public async Task ForTheRestCallToTheAgentQueueOnlyQueueIdsFromTheInputAreUsed()
            {
                _fixture.Customize<Response.AgentPool>(context => context.With(
                    x => x.Id, 115));

                _fixture.Customize<Response.DeployPhaseSnapshot>(context => context.With(
                    x => x.PhaseType, "agentBasedDeployment"));

                _fixture.Customize<Response.DeploymentInput>(context => context.With(
                    x => x.QueueId, 1234));

                var input = ReleaseCompletedInput();

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                await scan.GetCompletedReportAsync(input);

                await rest.Received(_fixture.Create<Response.Environment>().DeployPhasesSnapshot.Count())
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>());

                await rest
                    .Received()
                    .GetAsync(Arg.Is<IVstsRequest<Response.AgentQueue>>(r => r.Resource.Contains("1234")));
            }

            [Fact]
            public async Task DontCheckTasManagedAgentsForNonAgentBasedDeployments()
            {
                // Arrange
                _fixture
                    .Customize<Response.DeployPhaseSnapshot>(context =>
                        context.With(x => x.PhaseType, "machineGroupBasedDeployment"));

                var input = ReadInput("Completed", "Approved.json");

                var rest = Substitute.For<IVstsRestClient>();

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());


                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                await rest.DidNotReceive()
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>());
                Assert.Null(report.UsesManagedAgentsOnly);
            }

            [Fact]
            public async Task IfQueueIdResultsInUnmanagedPoolIdThenFalse()
            {
                // This is most definetely not an managed pool id.
                _fixture
                    .Customize<Response.AgentPool>(context => context.With(x => x.Id, 543));
                _fixture.Customize<Response.DeployPhaseSnapshot>(context =>
                    context.With(x => x.PhaseType, "agentBasedDeployment"));

                var input = ReleaseCompletedInput();

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.False(report.UsesManagedAgentsOnly);
            }

            [Fact]
            public async Task AllArtifactAreFromBuild()
            {
                // Arrange
                var artifacts = new[]
                {
                    new Response.ArtifactReference() {Alias = "Rianne", Type = "Build"}
                };

                _fixture
                    .Customize<Response.Release>(context => context.With(x => x.Artifacts, artifacts));

                var input = ReadInput("Completed", "Approved.json");

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.True(report.AllArtifactsAreFromBuild);
            }

            [Fact]
            public async Task ShouldReturnFalseIfArtifactsAreNotFromTypeBuild()
            {
                // Arrange
                var artifacts = new[]
                {
                    new Response.ArtifactReference() {Alias = "Rianne2", Type = "repository"}
                };

                _fixture
                    .Customize<Response.Release>(context => context.With(x => x.Artifacts, artifacts));

                var input = ReadInput("Completed", "Approved.json");

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.False(report.AllArtifactsAreFromBuild);
            }

            [Fact]
            public async Task ShouldReturnFalseIfArtifactsCountEqualsZero()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved2.json");

                var rest = Substitute.For<IVstsRestClient>();
                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>())
                    .Returns(_fixture.Create<Response.AgentQueue>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Release>>())
                    .Returns(_fixture.Create<Response.Release>());

                rest
                    .GetAsync(Arg.Any<IVstsRequest<Response.Environment>>())
                    .Returns(_fixture.Create<Response.Environment>());

                // Act
                var scan = new ReleaseDeploymentScan(rest);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.False(report.AllArtifactsAreFromBuild);
            }

            [Fact]
            public async Task NoInformationIfReleaseInformationIsUnavailable()
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
                client.GetAsync(Arg.Any<IVstsRequest<Response.AgentQueue>>()).Returns(_fixture.Create<Response.AgentQueue>());

                var scan = new ReleaseDeploymentScan(client);
                var report = await scan.GetCompletedReportAsync(input);

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
            public async Task ShouldReturnIdAndUrlWhenReleaseHasChangeTag()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(
                    x => x.With(
                        a => a.Tags, new[] { "C000123456 [00aa0a00]", "Random tag" }));

                // Act
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.Equal("C000123456", report.SM9ChangeId);
                Assert.Equal(new Uri($"http://itsm.somecompany.nl/SM/index.do?ctx=docEngine&file=cm3r&query=number%3D%22" +
                    $"C000123456%22&action=&title=Change%20Request%20Details&queryHash=00aa0a00"), report.SM9ChangeUrl);
            }

            [Fact]
            public async Task ShouldReturnNullForReleaseWithNoSm9ChangeTag()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(
                    x => x.With(
                        a => a.Tags, new[] { "12345", "Random tag" }));

                // Act
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.Null(report.SM9ChangeId);
                Assert.Null(report.SM9ChangeUrl);
            }

            [Fact]
            public async Task ShouldReturnSolelyIdForReleaseWithNoHashInSm9ChangeTag()
            {
                // Arrange
                var input = ReadInput("Completed", "Approved.json");
                _fixture.Customize<Response.Release>(
                    x => x.With(
                        a => a.Tags, new[] { "Change=C000123456 but no hash", "Random tag" }));

                // Act
                var client = new FixtureClient(_fixture);
                var scan = new ReleaseDeploymentScan(client);
                var report = await scan.GetCompletedReportAsync(input);

                // Assert
                Assert.Equal("C000123456", report.SM9ChangeId);
                Assert.Null(report.SM9ChangeUrl);
            }
        }

        private static JObject ReadInput(string eventType, string file)
        {
            var dir = Path.Join("Assets", "ReleaseDeployment", eventType);
            return JObject.Parse(File.ReadAllText(Path.Join(dir, file)));
        }
    }
}