using System;
using System.Linq;
using AutoFixture;
using Shouldly;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Hooks : IClassFixture<Hooks.HookFixture>
    {
        private readonly HookFixture _fixture;

        public Hooks(HookFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void QuerySubscriptions()
        {
            var subscriptions = _fixture.Client.Get(Requests.Hooks.Subscriptions());
            subscriptions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));
        }

        [Fact]
        public async Task BuildCompleted()
        {
            var body = Requests.Hooks.Add.BuildCompleted(
                _fixture.AccountName,
                _fixture.AccountKey,
                _fixture.QueueName,
                _fixture.ProjectId
            );

            var hook = await _fixture.Client.PostAsync(Requests.Hooks.AddHookSubscription(), body);
            hook.Id.ShouldNotBeNullOrEmpty();

            await _fixture.Client.DeleteAsync(Requests.Hooks.Subscription(hook.Id));
        }

        [Fact]
        public async Task GitPushed()
        {
            var body = Requests.Hooks.Add.GitPushed(
                _fixture.AccountName,
                _fixture.AccountKey,
                _fixture.QueueName,
                _fixture.ProjectId
            );

            var hook = await _fixture.Client.PostAsync(Requests.Hooks.AddHookSubscription(), body);
            hook.Id.ShouldNotBeNullOrEmpty();

            await _fixture.Client.DeleteAsync(Requests.Hooks.Subscription(hook.Id));
        }

        [Fact]
        public async Task GitPullRequestCreated()
        {
            var body = Requests.Hooks.Add.GitPullRequestCreated(
                _fixture.AccountName,
                _fixture.AccountKey,
                _fixture.QueueName,
                _fixture.ProjectId
            );

            var hook = await _fixture.Client.PostAsync(Requests.Hooks.AddHookSubscription(), body);
            hook.Id.ShouldNotBeNullOrEmpty();
            hook.PublisherInputs.PullrequestCreatedBy.ShouldNotBeNull();

            await _fixture.Client.DeleteAsync(Requests.Hooks.Subscription(hook.Id));
        }

        [Fact]
        public async Task ReleaseDeploymentCompleted()
        {
            var body = Requests.Hooks.Add.ReleaseDeploymentCompleted(
                _fixture.AccountName,
                _fixture.AccountKey,
                _fixture.QueueName,
                _fixture.ProjectId
            );

            var hook = await _fixture.Client.PostAsync(Requests.Hooks.AddReleaseManagementSubscription(), body);
            hook.Id.ShouldNotBeNullOrEmpty();

            await _fixture.Client.DeleteAsync(Requests.Hooks.Subscription(hook.Id));
        }

        public class HookFixture : IDisposable
        {
            public string AccountKey { get; } = "01234156789123456784564560123415678912345678456456123456789123456";
            public string AccountName { get; }
            public string QueueName { get; } = "queuename";
            public string ProjectId { get; }

            public IVstsRestClient Client { get; }

            public HookFixture()
            {
                var config = new TestConfig();

                Client = new VstsRestClient(config.Organization, config.Token);
                ProjectId = Client.Get(Requests.Project.Projects()).Single(p => p.Name == config.Project).Id;

                var fixture = new Fixture();
                AccountName = fixture.Create("integration-test-hook");
            }

            public void Dispose()
            {
                // Make sure all hooks from this test run are properly deleted.
                Client
                    .Get(Requests.Hooks.Subscriptions())
                    .ShouldNotContain(x => x.ConsumerInputs.AccountName == AccountName);
            }
        }
    }
}
