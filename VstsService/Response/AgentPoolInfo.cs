using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class AgentPoolInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsHosted { get; set; }

        public int Size { get; set; }

        public string PoolType { get; set; }

        public Guid Scope { get; set; }
    }
}