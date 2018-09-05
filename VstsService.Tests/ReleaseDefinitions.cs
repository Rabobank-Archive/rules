using Xunit;
using Shouldly;
using System.Net;
using System.Linq;

namespace SecurePipelineScan.VstsService.Tests
{
    public class Release : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public Release(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryReleaseDefinitions()
        {
            var definitions = client.Execute(Requests.Release.Definitions(config.Project));

            definitions.StatusCode.ShouldBe(HttpStatusCode.OK);
            definitions.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryReleaseDefinitionDetails()
        {
            var definition = client.Execute(Requests.Release.Definition(config.Project, "2"));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Name.ShouldBe("demo SOx");
        }

        [Fact]
        public void ReleaseWithApproval()
        {
            const string id = "616";

            var release = client.Execute(Requests.Release.Releases("TAS", id));
            release.ErrorMessage.ShouldBeNull();

            release.StatusCode.ShouldBe(HttpStatusCode.OK);
            release.Data.Id.ShouldBe(id);
            release.Data.Environments.ShouldNotBeEmpty();

            var env = release.Data.Environments.First();
            env.Id.ShouldNotBeNullOrEmpty();
            env.PreDeployApprovals.ShouldNotBeEmpty();
            env.DeploySteps.ShouldNotBeEmpty();

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
        }
    }
}
