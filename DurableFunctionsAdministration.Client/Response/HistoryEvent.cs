using System;

namespace DurableFunctionsAdministration.Client.Response
{
    public class HistoryEvent
    {
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string FunctionName { get; set; }
    }
}