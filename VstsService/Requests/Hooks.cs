using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Hooks
    {
        public static IVstsRequest<Multiple<Hook>> Subscriptions()
        {
            return new VstsRequest<Multiple<Hook>>(
                $"_apis/hooks/subscriptions", new Dictionary<string, object>
                {
                    {"api-version", "5.0-preview.1"}
                });
        }

        public static class Add
        {
            public static Body BuildCompleted(string accountName, string accountKey,
                string queueName, string projectId)
            {
                return new Body
                {
                    ConsumerActionId = "enqueue",
                    ConsumerId = "azureStorageQueue",
                    ConsumerInputs = new ConsumerInputs
                    {
                        AccountName = accountName,
                        AccountKey = accountKey,
                        QueueName = queueName,
                        VisiTimeout = "0",
                        Ttl = "604800",
                    },
                    EventType = "build.complete",
                    PublisherId = "tfs",
                    PublisherInputs = new BuildCompletePublisherInputs
                    {
                        DefinitionName = "",
                        ProjectId = projectId,
                        BuildStatus = "",
                    },
                    ResourceVersion = "1.0",
                    Scope = 1,
                };
            }

            public static Body GitPullRequestCreated(string accountName, string accountKey, string queueName, string projectId) =>
                new Body
                {
                    ConsumerActionId = "enqueue",
                    ConsumerId = "azureStorageQueue",
                    ConsumerInputs = new ConsumerInputs
                    {
                        AccountName = accountName,
                        AccountKey = accountKey,
                        QueueName = queueName,
                        VisiTimeout = "0",
                        Ttl = "604800",
                    },
                    EventType = "git.pullrequest.created",
                    PublisherId = "tfs",
                    PublisherInputs = new PullRequestCreatedPublisherInputs
                    {
                        ProjectId = projectId,
                        Repository = "",
                        Branch = "",
                        PullrequestCreatedBy = "",
                        PullrequestReviewersContains = ""
                    },
                    ResourceVersion = "1.0",
                    Scope = 1,
                };

            public static Body GitPushed(string accountName, string accountKey, string queueName, string projectId) =>
                new Body
                {
                    ConsumerActionId = "enqueue",
                    ConsumerId = "azureStorageQueue",
                    ConsumerInputs = new ConsumerInputs
                    {
                        AccountName = accountName,
                        AccountKey = accountKey,
                        QueueName = queueName,
                        VisiTimeout = "0",
                        Ttl = "604800",
                    },
                    EventType = "git.push",
                    PublisherId = "tfs",
                    PublisherInputs = new GitPushPublisherInputs
                    {
                        ProjectId = projectId,
                        Repository = "",
                        Branch = "",
                        PushedBy = "",
                    },
                    ResourceVersion = "1.0",
                    Scope = 1,
                };

            public static Body ReleaseDeploymentCompleted(string accountName, string accountKey, string queueName, string projectId) =>
                new Body
                {
                    ConsumerActionId = "enqueue",
                    ConsumerId = "azureStorageQueue",
                    ConsumerInputs = new ConsumerInputs
                    {
                        AccountName = accountName,
                        AccountKey = accountKey,
                        QueueName = queueName,
                        VisiTimeout = "0",
                        Ttl = "604800",
                    },
                    EventType = "ms.vss-release.deployment-completed-event",
                    PublisherId = "rm",
                    PublisherInputs = new ReleaseDeploymentCreatedPublisherInputs
                    {
                        ProjectId = projectId,
                        ReleaseDefinitionId = "",
                        ReleaseEnvironmentId = "",
                        ReleaseEnvironmentStatus = "",
                    },
                    ResourceVersion = "3.0-preview.1",
                    Scope = 1,
                };

            public class ConsumerInputs
            {
                public string AccountName { get; set; }
                public string AccountKey { get; set; }
                public string QueueName { get; set; }
                public string VisiTimeout { get; set; }
                public string Ttl { get; set; }
            }

            public abstract class PublisherInputs
            {
                public string ProjectId { get; set; }
            }

            public class Body
            {
                public string ConsumerActionId { get; set; }
                public string ConsumerId { get; set; }
                public ConsumerInputs ConsumerInputs { get; set; }
                public string EventType { get; set; }
                public string PublisherId { get; set; }
                public PublisherInputs PublisherInputs { get; set; }
                public string ResourceVersion { get; set; }
                public int Scope { get; set; }
            }

            private class ReleaseDeploymentCreatedPublisherInputs : PublisherInputs
            {
                public string ReleaseDefinitionId { get; set; }
                public string ReleaseEnvironmentId { get; set; }
                public string ReleaseEnvironmentStatus { get; set; }
            }

            private class PullRequestCreatedPublisherInputs : PublisherInputs
            {
                public string Repository { get; set; }
                public string Branch { get; set; }
                public string PullrequestCreatedBy { get; set; }
                public string PullrequestReviewersContains { get; set; }
            }

            private class GitPushPublisherInputs : PublisherInputs
            {
                public string Repository { get; set; }
                public string Branch { get; set; }
                public string PushedBy { get; set; }
            }

            private class BuildCompletePublisherInputs : PublisherInputs
            {
                public string DefinitionName { get; set; }
                public string BuildStatus { get; set; }
            }
        }
        
        public static IVstsRequest<Hook> Subscription(string id) => new VstsRequest<Hook>($"_apis/hooks/subscriptions/{id}", new Dictionary<string, object>
        {
            {"api-version","5.0-preview.1"}
        });
        public static IVstsRequest<Add.Body, Hook> AddHookSubscription() => new VstsRequest<Add.Body, Hook>($"_apis/hooks/subscriptions", new Dictionary<string, object>
        {
            {"api-version","5.0-preview.1"}
        });
        public static IVstsRequest<Add.Body, Hook> AddReleaseManagementSubscription() => new VsrmRequest<Add.Body, Hook>($"_apis/hooks/subscriptions", new Dictionary<string, object>
        {
            {"api-version","5.0-preview.1"}
        });
    }
}