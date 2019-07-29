using System;
using System.Collections.Generic;
using AzDoCompliancy.CustomStatus.Converter;
using Newtonsoft.Json;

namespace DurableFunctionsAdministration.Client.Response
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrchestrationInstance
    {
        public string InstanceId { get; set; }
        public string Name { get; set; }
        public string RuntimeStatus { get; set; }

        [JsonConverter(typeof(CustomStatusConverter))]
        public AzDoCompliancy.CustomStatus.CustomStatusBase CustomStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public IList<HistoryEvent> HistoryEvents { get; }
    }
}