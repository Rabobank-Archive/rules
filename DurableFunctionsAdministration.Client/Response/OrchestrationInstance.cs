using System;
using System.Collections.Generic;

namespace DurableFunctionsAdministration.Client.Response
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrchestrationInstance
    {
        public string InstanceId { get; set; }
        public string RuntimeStatus { get; set; }
        public AzDoCompliancy.CustomStatus.ICustomStatus CustomStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public List<HistoryEvent> HistoryEvents { get; set; }
    }
}