using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Hooks
    {
        private static readonly IDictionary<string, object> CommonApiVersion = new Dictionary<string, object>
            {
                {"api-version", "5.0-preview.1"}
            };

        public static IEnumerableRequest<Hook> Subscriptions() =>
            new VstsRequest<Hook>($"_apis/hooks/subscriptions", CommonApiVersion).AsEnumerable();

        public static class Add
        {
            public static Body BuildCompleted(string accountName, string accountKey,
                string queueName, string projectId)
            {
                var publisherInputs = new BuildCompletePublisherInputs
                {
                    DefinitionName = "",
                    ProjectId = projectId,
                    BuildStatus = "",
                };
                return NewBody(accountName, accountKey, queueName, "build.complete", "tfs", publisherInputs, "1.0");
            }

            public static Body GitPullRequestCreated(string accountName, string accountKey, string queueName, string projectId)
            {
                var publisherInputs = new PullRequestCreatedPublisherInputs
                {
                    ProjectId = projectId,
                    Repository = "",
                    Branch = "",
                    PullrequestCreatedBy = "",
                    PullrequestReviewersContains = ""
                };
                return NewBody(accountName, accountKey, queueName, "git.pullrequest.created", "tfs", publisherInputs, "1.0");
            }

            public static Body GitPushed(string accountName, string accountKey, string queueName, string projectId)
            {
                var publisherInputs = new GitPushPublisherInputs
                {
                    ProjectId = projectId,
                    Repository = "",
                    Branch = "",
                    PushedBy = "",
                };
                return NewBody(accountName, accountKey, queueName, "git.push", "tfs", publisherInputs, "1.0");
            }

            public static Body ReleaseDeploymentCompleted(string accountName, string accountKey, string queueName, string projectId)
            {
                var publisherInputs = new ReleaseDeploymentCreatedPublisherInputs
                {
                    ProjectId = projectId,
                    ReleaseDefinitionId = "",
                    ReleaseEnvironmentId = "",
                    ReleaseEnvironmentStatus = "",
                };
                return NewBody(accountName, accountKey, queueName, "ms.vss-release.deployment-completed-event", 
                    "rm", publisherInputs, "3.0-preview.1");
            }                    

            public class ConsumerInputs
            {
                public string AccountName { get; set; }
                public string AccountKey { get; set; }
                public string QueueName { get; set; }
                public string VisiTimeout { get; set; }
                public string Ttl { get; set; }
            }

            public class PublisherInputs
            {
                protected PublisherInputs()
                {
                }
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

            public static Body NewBody(string accountName, string accountKey, string queueName, string eventType, 
                string publisherId, PublisherInputs publisherInputs, string resourceVersion)
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
                    EventType = eventType,
                    PublisherId = publisherId,
                    PublisherInputs = publisherInputs,
                    ResourceVersion = resourceVersion,
                    Scope = 1,
                };
            }
        }
        
        public static IVstsRequest<Hook> Subscription(string id) => 
            new VstsRequest<Hook>($"_apis/hooks/subscriptions/{id}", CommonApiVersion);
        public static IVstsRequest<Add.Body, Hook> AddHookSubscription() => 
            new VstsRequest<Add.Body, Hook>($"_apis/hooks/subscriptions", CommonApiVersion);
        public static IVstsRequest<Add.Body, Hook> AddReleaseManagementSubscription() => 
            new VsrmRequest<Add.Body, Hook>($"_apis/hooks/subscriptions", CommonApiVersion);
    }
}