using System;

namespace DurableFunctionsAdministration.Client.Response
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrchestrationInstance
    {
        public string InstanceId { get; set; }
        public string RuntimeStatus { get; set; }
        public object CustomStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}