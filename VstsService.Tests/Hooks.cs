using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Requests = SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Linq;
using System.Net;
using RestSharp;
using Xunit;

namespace VstsService.Tests
{
    [Trait("category", "integration")]
    public class Hooks : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public Hooks(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        /// <summary>
        /// Test if query, add and delete subscriptions work
        /// </summary>
        [Fact]
        public void QueryAddDelete_BuildComplete_Subscription()
        {
            var subscribtionsBefore = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Get(Requests.Project.Projects()).Single(p => p.Name == config.Project).Id;

            var addHook = client.Post(Requests.Hooks.Add.BuildCompleted(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Id;

            var subscribtionsAfter = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.Count.ShouldBeLessThan(subscribtionsAfter.Count);
            client.Delete(Requests.Hooks.Subscription(addedHookId));

            var subscribtionsFinal = client.Get(Requests.Hooks.Subscriptions());
            subscribtionsFinal.Count.ShouldBe(subscribtionsBefore.Count);
        }

        [Fact]
        public void QueryAddDelete_GitPushed_Subscription()
        {
            var subscribtionsBefore = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Get(Requests.Project.Projects()).Single(p => p.Name == config.Project).Id;

            var addHook = client.Post(Requests.Hooks.Add.GitPushed(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Id;

            var subscribtionsAfter = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.Count.ShouldBeLessThan(subscribtionsAfter.Count);

            client.Delete(Requests.Hooks.Subscription(addedHookId));


            var subscribtionsFinal = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsFinal.Count.ShouldBe(subscribtionsBefore.Count);
        }

        [Fact]
        public void QueryAddDelete_GitPullRequestCreated_Subscription()
        {
            var subscribtionsBefore = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Get(Requests.Project.Projects()).Single(p => p.Name == config.Project).Id;

            var addHook = client.Post(Requests.Hooks.Add.GitPullRequestCreated(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Id;

            var subscribtionsAfter = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.Count.ShouldBeLessThan(subscribtionsAfter.Count);
            client.Delete(Requests.Hooks.Subscription(addedHookId));


            var subscribtionsFinal = client.Get(Requests.Hooks.Subscriptions());
            subscribtionsFinal.Count.ShouldBe(subscribtionsBefore.Count);
        }

        [Fact]
        public void QueryAddDelete_ReleaseDeploymentCompleted_Subscription()
        {
            var subscribtionsBefore = client.Get(Requests.Hooks.Subscriptions());
            subscribtionsBefore.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Get(Requests.Project.Projects()).Single(p => p.Name == config.Project).Id;

            var addHook = client.Post(Requests.Hooks.Add.ReleaseDeploymentCompleted(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Id;

            var subscribtionsAfter = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsBefore.Count.ShouldBeLessThan(subscribtionsAfter.Count);

            client.Delete(Requests.Hooks.Subscription(addedHookId));


            var subscribtionsFinal = client.Get(Requests.Hooks.Subscriptions());

            subscribtionsFinal.Count.ShouldBe(subscribtionsBefore.Count);
        }
    }
}