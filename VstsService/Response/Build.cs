using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class Build
    {
        public Project Project { get; set; }
        public int Id { get; set; }
        public Definition Definition { get; set; }
        public string Result { get; set; }
        public string BuildNumber { get; set; }
        public DateTime QueueTime { get; set; }
        public DateTime StartTime { get; set; }
        public AgentQueue Queue { get; set; }
        public RequestedFor RequestedFor { get; set; }
    }
}