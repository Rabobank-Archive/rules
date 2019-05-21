namespace SecurePipelineScan.VstsService.Requests
{
    public static class DistributedTask
    {
        public static IVstsRequest<Response.Multiple<Response.AgentPoolInfo>> OrganizationalAgentPools()
        {
            return new VstsRequest<Response.Multiple<Response.AgentPoolInfo>>($"_apis/distributedtask/pools");
        }

        public static IVstsRequest<Response.AgentPoolInfo> AgentPool(int id)
        {
            return new VstsRequest<Response.AgentPoolInfo>($"_apis/distributedtask/pools/{id}");
        }

        public static IVstsRequest<Response.Multiple<Response.AgentStatus>> AgentPoolStatus(int id)
        {
            return new VstsRequest<Response.Multiple<Response.AgentStatus>>($"_apis/distributedtask/pools/{id}/agents?includeCapabilities=false&includeAssignedRequest=true");
        }

        public static IVstsRequest<Response.Multiple<Response.Task>> Tasks()
        {
            return new VstsRequest<Response.Multiple<Response.Task>>($"_apis/distributedtask/tasks");
        }

        public static IVstsRequest<Response.AgentQueue> AgentQueue(string project, int id)
        {
            return new VstsRequest<Response.AgentQueue>($"/{project}/_apis/distributedtask/queues/{id}");
        }
    }
}