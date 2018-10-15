using Xunit;
using Shouldly;
using System.Net;
using System.Linq;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Release : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient client;

        public Release(TestConfig config)
        {
            client = new VstsRestClient(config.Organization, config.Token);
        }
    

        [Fact]
        public void ReleaseWithApproval()
        {
            const string id = "978"; // <-- just some release, may be gone in future due to retention policy which sucks for reporting

            var release = client.Execute(Requests.Release.Releases("TAS", id));
            release.ErrorMessage.ShouldBeNull();

            release.StatusCode.ShouldBe(HttpStatusCode.OK);
            release.Data.Id.ShouldBe(id);
            release.Data.Environments.ShouldNotBeEmpty();

            var env = release.Data.Environments.First();
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
