using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class DistributedTask
    {
        public static IEnumerableRequest<AgentPoolInfo> OrganizationalAgentPools() => 
            new VstsRequest<AgentPoolInfo>("_apis/distributedtask/pools").AsEnumerable();

        public static IVstsRequest<AgentPoolInfo> AgentPool(int id) => 
            new VstsRequest<AgentPoolInfo>($"_apis/distributedtask/pools/{id}");

        public static IEnumerableRequest<AgentStatus> AgentPoolStatus(int id) =>
            new VstsRequest<AgentStatus>($"_apis/distributedtask/pools/{id}/agents", new Dictionary<string, object>
            {
                {"includeCapabilities", "false"},
                {"includeAssignedRequest", "true"}
            }).AsEnumerable();

        public static IEnumerableRequest<Task> Tasks() => 
            new VstsRequest<Task>("_apis/distributedtask/tasks").AsEnumerable();

        public static IVstsRequest<AgentQueue> AgentQueue(string project, int id) => 
            new VstsRequest<AgentQueue>($"/{project}/_apis/distributedtask/queues/{id}");
    }
}