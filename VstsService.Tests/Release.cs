using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Shouldly;
using System.Linq;
using Flurl.Http.Testing;
using Environment = SecurePipelineScan.VstsService.Response.Environment;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Release : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;
        private readonly string _project;

        private static readonly string ReleaseAssets = Path.Join("Assets", "Release");
        private static readonly string EnvironmentAssets = Path.Join(ReleaseAssets, "Environment");

        public Release(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
            _project = config.Project;
        }


        [Fact]
        public async Task ReleaseWithApproval()
        {
            const int id = 79;

            var response = File.ReadAllText(Path.Join(ReleaseAssets, "Approved.json"));

            var request = new VstsRequest<Response.Release>("/keeas");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 200, body: response);
                var client = new VstsRestClient("dummy", "pat");
                var release = await client.GetAsync(request);
                release.Id.ShouldBe(id);
                release.Environments.ShouldNotBeEmpty();
                release.Tags.ShouldNotBeEmpty();
                var env = release.Environments.Skip(1).First();
                env.Id.ShouldNotBe(0);
                env.PreDeployApprovals.ShouldNotBeEmpty();
                env.DeploySteps.ShouldNotBeEmpty();
                env.Name.ShouldNotBeNullOrEmpty();
                env.DeployPhasesSnapshot.ShouldNotBeEmpty();

                var phaseSnapshot = env.DeployPhasesSnapshot.First();
                phaseSnapshot.PhaseType.ShouldNotBeEmpty();
                phaseSnapshot.DeploymentInput.ShouldNotBeNull();
                phaseSnapshot.DeploymentInput.QueueId.ShouldNotBe(0);

                var deploy = env.DeploySteps.First();
                deploy.RequestedFor.ShouldNotBeNull();
                deploy.RequestedFor.Id.ShouldNotBeNull();
                deploy.LastModifiedBy.ShouldNotBeNull();

                var predeploy = env.PreDeployApprovals.First();
                predeploy.Status.ShouldNotBeNullOrEmpty();
                predeploy.ApprovalType.ShouldNotBeNullOrEmpty();
                predeploy.IsAutomated.ShouldBe(false);
                predeploy.ApprovedBy.ShouldNotBeNull();
                predeploy.ApprovedBy.DisplayName.ShouldNotBeNullOrEmpty();

                var conditions = env.Conditions.ToList();
                conditions.ShouldNotBeEmpty();

                var condition = conditions.First();
                condition.Result.ShouldBe(false);
                condition.Name.ShouldNotBeNullOrEmpty();
                condition.ConditionType.ShouldNotBeEmpty();
                condition.Value.ShouldNotBeNull();

                var artifact = release.Artifacts.First();
                artifact.Type.ShouldNotBeNull();
                artifact.Alias.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task QueryReleasesByPipelineAndStages()
        {
            var releases = _client.Get(Requests.ReleaseManagement.Releases(
                _project, "1", new string[] {"1","2"}.AsEnumerable(), "environments", "1-1-2019"));

            releases.ShouldNotBeNull();
            releases.ShouldNotBeEmpty();
            releases.SelectMany(r => r.Environments).ShouldNotBeNull();
            releases.SelectMany(r => r.Environments).ShouldNotBeEmpty();
        }

        [Fact]
        public async Task QueryReleasesByPipelineAndStagesWithContinuationToken()
        {
            var releases = _client.Get(Requests.ReleaseManagement.Releases(
                _project, "1", new string[] { "1", "2" }.AsEnumerable(), "environments", "1-1-2019", "0"));

            releases.ShouldNotBeNull();
            releases.ShouldNotBeEmpty();
            releases.SelectMany(r => r.Environments).ShouldNotBeNull();
            releases.SelectMany(r => r.Environments).ShouldNotBeEmpty();
        }

        [Fact]
        public async Task QueryEnvironment()
        {
            var environment = await _client.GetAsync(Requests.ReleaseManagement.Environment(_project, "5", "7"));

            environment.ShouldNotBeNull();
            environment.DeployPhasesSnapshot.ShouldNotBeEmpty();

            var snapshot = environment.DeployPhasesSnapshot.First();
            snapshot.WorkflowTasks.ShouldNotBeEmpty();

            var task = snapshot.WorkflowTasks.First();
            task.TaskId.ShouldNotBe(Guid.Empty);
            task.Inputs.ShouldNotBeEmpty();

            var preApprovalSnapshot = environment.PreApprovalsSnapshot;
            preApprovalSnapshot.ShouldNotBeNull();
            preApprovalSnapshot.ApprovalOptions.ShouldNotBeNull();
            preApprovalSnapshot.ApprovalOptions.ReleaseCreatorCanBeApprover.ShouldBeTrue();

            var approval = preApprovalSnapshot.Approvals.FirstOrDefault();
            approval.ShouldNotBeNull();
            approval.IsAutomated.ShouldBeFalse();
        }

        [Fact]
        public void RequestForMultipleContinuesUsingContinuationToken()
        {
            var releases = _client.Get(
                    new VsrmRequest<Response.Release>($"{_config.Project}/_apis/release/releases/",
                        new Dictionary<string, object>
                        {
                            {"$top", "2"}
                        }).AsEnumerable());
            releases.Count().ShouldBeGreaterThan(2);
        }

        [Fact]
        public async Task ConditionResultOnReleaseEnvironmentMustBeNullable()
        {
            var response = File.ReadAllText(Path.Join(EnvironmentAssets, "ConditionResultNull.json"));

            var request = new VstsRequest<Environment>("/keeas");

            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(status: 200, body: response);
                var client = new VstsRestClient("dummy", "pat");
                await client.GetAsync(request);
            }
        }
    }
}