using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class Hook
    {
        public string Id { get; set; }
        public Uri Url { get; set; }
        public string Status { get; set; }
        public string PublisherId { get; set; }
        public string EventType { get; set; }
        public object Subscriber { get; set; }
        public string ResourceVersion { get; set; }
        public string EventDescription { get; set; }
        public string ConsumerId { get; set; }
        public string ConsumerActionId { get; set; }
        public string ActionDescription { get; set; }

        public PublisherInputs PublisherInputs { get; set; }
        public ConsumerInputs ConsumerInputs { get; set; }
    }

    public class PublisherInputs
    {
        public string ProjectId { get; set; }
        public string ReleaseDefinitionId { get; set; }
        public string TfsSubscriptionId { get; set; }
        public string BuildStatus { get; set; }
        public string DefinitionName { get; set; }
        public string Branch { get; set; }
        public string NotificationType { get; set; }
        public string PullrequestCreatedBy { get; set; }
        public string PullrequestReviewersContains { get; set; }
        public string Repository { get; set; }
    }

    public class ConsumerInputs
    {
        public string AccountName { get; set; }
        public string QueueName { get; set; }
        public string Ttl { get; set; }
        public string VisiTimeout { get; set; }
        public string AccountKey { get; set; }
    }
}