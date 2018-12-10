using Xunit;
using Shouldly;
using System.Net;
using System.Linq;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Release : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;
        private readonly string _project;

        public Release(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
            _project = config.Project;
        }
    

        [Fact]
        public void ReleaseWithApproval()
        {
            const string id = "42"; // <-- just some release, may be gone in future due to retention policy which sucks for reporting

            var release = _client.Execute(Requests.Release.Releases(_project, id));
            release.ErrorMessage.ShouldBeNull();

            release.StatusCode.ShouldBe(HttpStatusCode.OK);
            release.Data.Id.ShouldBe(id);
            release.Data.Environments.ShouldNotBeEmpty();

            var env = release.Data.Environments.Skip(1).First();
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
            condition.Result.ShouldBeTrue();
            condition.Name.ShouldNotBeNullOrEmpty();
            condition.ConditionType.ShouldNotBeEmpty();
            condition.Value.ShouldNotBeNull();
        }
    }
}
