using System;
using System.IO;
using Xunit;
using Shouldly;
using System.Net;
using System.Linq;
using NSubstitute;
using RestSharp;
using Environment = SecurePipelineScan.VstsService.Response.Environment;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Release : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;
        private readonly string _project;
        
        private static readonly string EnvironmentAssets = Path.Join("Assets", "Release", "Environment");

        public Release(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
            _project = config.Project;
        }
    

        [Fact]
        public void ReleaseWithApproval()
        {
            const string id = "42"; // <-- just some release, may be gone in future due to retention policy which sucks for reporting

            var release = _client.Get(Requests.Release.Releases(_project, id));

            release.Id.ShouldBe(id);
            release.Environments.ShouldNotBeEmpty();

            var env = release.Environments.Skip(1).First();
            env.Id.ShouldNotBe(0);
            env.PreDeployApprovals.ShouldNotBeEmpty();
            env.DeploySteps.ShouldNotBeEmpty();
            env.Name.ShouldNotBeNullOrEmpty();

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

            var conditions = env.Conditions;
            conditions.ShouldNotBeEmpty();

            var condition = conditions.First();
            condition.Result.ShouldBe(true);
            condition.Name.ShouldNotBeNullOrEmpty();
            condition.ConditionType.ShouldNotBeEmpty();
            condition.Value.ShouldNotBeNull();
        }

        [Fact]
        public void QueryEnvironment()
        {
            var environment = _client.Get(Requests.Release.Environment(_project, "62", "115"));

            environment.ShouldNotBeNull();
            environment.DeployPhasesSnapshot.ShouldNotBeEmpty();

            var snapshot = environment.DeployPhasesSnapshot.First();
            snapshot.WorkflowTasks.ShouldNotBeEmpty();

            var task = snapshot.WorkflowTasks.First();
            task.TaskId.ShouldNotBe(Guid.Empty);
            task.Inputs.ShouldNotBeEmpty();
        }

        [Fact]
        public void ConditionResultOnReleaseEnvironmentMustBeNullable()
        {
            // Source: https://vsrm.dev.azure.com/somecompany/Investments/_apis/release/releases/7604/environments/57594
            // _client.Get(Requests.Release.Environment("Investments", "7604", "57594"));

            var response = EmptyResponse();
            response.Content = File.ReadAllText(Path.Join(EnvironmentAssets, "ConditionResultNull.json"));
            
            var client = new RestClient().SetupSerializer();
            client.Deserialize<Environment>(response).ThrowOnError();
        }

        private static IRestResponse EmptyResponse()
        {
            var response = Substitute.For<IRestResponse>();
            response.Request = Substitute.For<IRestRequest>();
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK;
            response.ResponseStatus = ResponseStatus.Completed;
            
            return response;
        }
    }
}
