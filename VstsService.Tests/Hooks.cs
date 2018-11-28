using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Requests = SecurePipelineScan.VstsService.Requests;
using Shouldly;
using System.Linq;
using System.Net;
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
            var subscribtionsBefore = client.Execute(Requests.Hooks.Subscriptions());

            subscribtionsBefore.StatusCode.ShouldBe(HttpStatusCode.OK);
            subscribtionsBefore.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Execute(Requests.Project.Projects()).Data.Value.Single(p => p.Name == config.Project).Id;

            var addHook = client.Execute(Requests.Hooks.Add.BuildCompleted(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.StatusCode.ShouldBe(HttpStatusCode.OK);
            addHook.Data.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Data.Id;

            var subscribtionsAfter = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsAfter.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsBefore.Data.Count.ShouldBeLessThan(subscribtionsAfter.Data.Count);

            var deleteHook = client.Execute(Requests.Hooks.Delete(addedHookId));

            deleteHook.ResponseStatus.ShouldBe(RestSharp.ResponseStatus.Completed);

            var subscribtionsFinal = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsFinal.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsFinal.Data.Count.ShouldBe(subscribtionsBefore.Data.Count);
        }

        [Fact]
        public void QueryAddDelete_GitPushed_Subscription()
        {
            var subscribtionsBefore = client.Execute(Requests.Hooks.Subscriptions());

            subscribtionsBefore.StatusCode.ShouldBe(HttpStatusCode.OK);
            subscribtionsBefore.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Execute(Requests.Project.Projects()).Data.Value.Single(p => p.Name == config.Project).Id;

            var addHook = client.Execute(Requests.Hooks.Add.GitPushed(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.StatusCode.ShouldBe(HttpStatusCode.OK);
            addHook.Data.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Data.Id;

            var subscribtionsAfter = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsAfter.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsBefore.Data.Count.ShouldBeLessThan(subscribtionsAfter.Data.Count);

            var deleteHook = client.Execute(Requests.Hooks.Delete(addedHookId));

            deleteHook.ResponseStatus.ShouldBe(RestSharp.ResponseStatus.Completed);

            var subscribtionsFinal = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsFinal.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsFinal.Data.Count.ShouldBe(subscribtionsBefore.Data.Count);
        }

        [Fact]
        public void QueryAddDelete_GitPullRequestCreated_Subscription()
        {
            var subscribtionsBefore = client.Execute(Requests.Hooks.Subscriptions());

            subscribtionsBefore.StatusCode.ShouldBe(HttpStatusCode.OK);
            subscribtionsBefore.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Execute(Requests.Project.Projects()).Data.Value.Single(p => p.Name == config.Project).Id;

            var addHook = client.Execute(Requests.Hooks.Add.GitPullRequestCreated(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.StatusCode.ShouldBe(HttpStatusCode.OK);
            addHook.Data.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Data.Id;

            var subscribtionsAfter = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsAfter.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsBefore.Data.Count.ShouldBeLessThan(subscribtionsAfter.Data.Count);

            var deleteHook = client.Execute(Requests.Hooks.Delete(addedHookId));

            deleteHook.ResponseStatus.ShouldBe(RestSharp.ResponseStatus.Completed);

            var subscribtionsFinal = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsFinal.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsFinal.Data.Count.ShouldBe(subscribtionsBefore.Data.Count);
        }

        [Fact]
        public void QueryAddDelete_ReleaseDeploymentCompleted_Subscription()
        {
            var subscribtionsBefore = client.Execute(Requests.Hooks.Subscriptions());

            subscribtionsBefore.StatusCode.ShouldBe(HttpStatusCode.OK);
            subscribtionsBefore.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Id));

            string accountName = "rabovstslog";
            string accountKey = "01234156789123456784564560123415678912345678456456123456789123456";
            string queueName = "queuename";
            var projectId = client.Execute(Requests.Project.Projects()).Data.Value.Single(p => p.Name == config.Project).Id;

            var addHook = client.Execute(Requests.Hooks.Add.ReleaseDeploymentCompleted(
                accountName,
                accountKey,
                queueName,
                projectId
                ));

            addHook.StatusCode.ShouldBe(HttpStatusCode.OK);
            addHook.Data.Id.ShouldNotBeNullOrEmpty();

            var addedHookId = addHook.Data.Id;

            var subscribtionsAfter = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsAfter.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsBefore.Data.Count.ShouldBeLessThan(subscribtionsAfter.Data.Count);

            var deleteHook = client.Execute(Requests.Hooks.Delete(addedHookId));

            deleteHook.ResponseStatus.ShouldBe(RestSharp.ResponseStatus.Completed);

            var subscribtionsFinal = client.Execute(Requests.Hooks.Subscriptions());
            subscribtionsFinal.StatusCode.ShouldBe(HttpStatusCode.OK);

            subscribtionsFinal.Data.Count.ShouldBe(subscribtionsBefore.Data.Count);
        }
    }
}