using System;
using System.IO;
using Xunit;
using Shouldly;
using System.Net;
using System.Linq;
using NSubstitute;
using RestSharp;
using SecurePipelineScan.VstsService.Response;
using Environment = SecurePipelineScan.VstsService.Response.Environment;

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
        public void ReleaseWithApproval()
        {
            const string id = "79";
            
            // var release = _client.Get(Requests.Release.Releases(_project, id));          
            var release = MockClientResponse<Response.Release>(Path.Join(ReleaseAssets, "Approved.json"));
            release.Id.ShouldBe(id);
            release.Environments.ShouldNotBeEmpty();
            release.Tags.ShouldNotBeEmpty();

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
            condition.Result.ShouldBe(false);
            condition.Name.ShouldNotBeNullOrEmpty();
            condition.ConditionType.ShouldNotBeEmpty();
            condition.Value.ShouldNotBeNull();

            var artifact = release.Artifacts.First();
            artifact.Type.ShouldNotBeNull();
            artifact.Alias.ShouldNotBeNull();           
        }


        [Fact]
        public void QueryEnvironment()
        {
            var environment = _client.Get(Requests.Release.Environment(_project, "5", "7"));

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
            var releases = _client.Get(new VsrmRequest<Multiple<Response.Release>>($"{_config.Project}/_apis/release/releases/?$top=2"));
            releases.Count().ShouldBeGreaterThan(2);
        }

        [Fact]
        public void ConditionResultOnReleaseEnvironmentMustBeNullable()
        {
            /*
             * First test to use json file for test deserialization.
             *   Source: https://vsrm.dev.azure.com/somecompany/Investments/_apis/release/releases/7604/environments/57594
             *   _client.Get(Requests.Release.Environment("Investments", "7604", "57594"));
             */
            MockClientResponse<Environment>(Path.Join(EnvironmentAssets, "ConditionResultNull.json"));
        }
        
        private static T MockClientResponse<T>(string path)
        {
            var response = MockResponse(File.ReadAllText(path));
            var client = new RestClient().SetupSerializer();
            
            return client.Deserialize<T>(response).ThrowOnError().Data;
        }

        private static IRestResponse MockResponse(string content)
        {
            var response = Substitute.For<IRestResponse>();
            response.Request = Substitute.For<IRestRequest>();
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK;
            response.ResponseStatus = ResponseStatus.Completed;
            response.Content = content;

            return response;
        }
    }
}