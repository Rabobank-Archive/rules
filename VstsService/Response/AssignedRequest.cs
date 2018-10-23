using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class AssignedRequest
    {
        public int RequestId { get; set; }

        public DateTime QueueTime { get; set; }

        public DateTime AssignTime { get; set; }

        public DateTime ReceiveTime { get; set; }

        public DateTime LockedUntil { get; set; }

        public string ServiceOwner { get; set; }

        public string HostId { get; set; }

        public string ScopeId { get; set; }

        public string PlanType { get; set; }

        public string PlanId { get; set; }

        public string JobId { get; set; }

        public List<string> Demands { get; set; }

        public int PoolId { get; set; }

        public Definition Definition { get; set; }
    }
}