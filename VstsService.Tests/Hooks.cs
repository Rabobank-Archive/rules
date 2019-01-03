using System.Linq;
using AutoFixture;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Hooks : IClassFixture<TestConfig>
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly string _accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
        private readonly string _queueName = "queuename";
        private readonly string _projectId;
        private readonly IVstsRestClient client;

        public Hooks(TestConfig config)
        {
            client = new VstsRestClient(config.Organization, config.Token);
            _projectId = client.Get(Requests.Project.Projects()).Single(p => p.Name == config.Project).Id;
        }

        [Fact]
        public void QuerySubscriptions()
        {
            var subscriptions = client.Get(Requests.Hooks.Subscriptions());
            subscriptions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));
        }

        [Fact]
        public void AddAndDelete_BuildComplete_Subscription()
        {
            var accountName = _fixture.Create("integration-test-hook");
            var body = Requests.Hooks.Add.BuildCompleted(
                accountName,
                _accountKey,
                _queueName,
                _projectId
            );
            
            var hook = CreateHook(body);
            DeleteHook(hook, accountName);
        }

        [Fact]
        public void QueryAddDelete_GitPushed_Subscription()
        {
            var accountName = _fixture.Create("integration-test-hook");
            var body = Requests.Hooks.Add.GitPushed(
                accountName,
                _accountKey,
                _queueName,
                _projectId
            );

            var hook = CreateHook(body);
            DeleteHook(hook, accountName);
        }

        [Fact]
        public void QueryAddDelete_GitPullRequestCreated_Subscription()
        {
            var accountName = _fixture.Create("integration-test-hook");
            var body = Requests.Hooks.Add.GitPullRequestCreated(
                accountName,
                _accountKey,
                _queueName,
                _projectId
            );

            var hook = CreateHook(body);
            DeleteHook(hook, accountName);
        }

        [Fact]
        public void QueryAddDelete_ReleaseDeploymentCompleted_Subscription()
        {
            var accountName = _fixture.Create("integration-test-hook");
            var body = Requests.Hooks.Add.ReleaseDeploymentCompleted(
                accountName,
                _accountKey,
                _queueName,
                _projectId
            );

            var hook = CreateHook(body);
            DeleteHook(hook, accountName);
        }

        private Hook CreateHook(IVstsPostRequest<Hook> body)
        {
            var hook = client.Post(body);
            hook.Id.ShouldNotBeNullOrEmpty();

            var subscriptions = client.Get(Requests.Hooks.Subscriptions());
            subscriptions.ShouldContain(_ => _.Id == hook.Id);
            
            return hook;
        }

        private void DeleteHook(Hook hook, string accountName)
        {
            client.Delete(Requests.Hooks.Subscription(hook.Id));
            var subscriptions = client.Get(Requests.Hooks.Subscriptions());
            subscriptions.ShouldNotContain(_ => _.ConsumerInputs.AccountName == accountName);
            subscriptions.ShouldNotContain(_ => _.Id == hook.Id);
        }
    }
}
