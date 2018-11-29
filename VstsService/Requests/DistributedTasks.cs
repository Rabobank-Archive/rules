using RestSharp;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class DistributedTask
    {
        public static IVstsRestRequest<Response.Multiple<Response.AgentPoolInfo>> OrganizationalAgentPools()
        {
            return new VstsRestRequest<Response.Multiple<Response.AgentPoolInfo>>($"_apis/distributedtask/pools",Method.GET);
        }

        public static IVstsRestRequest<Response.AgentPoolInfo> AgentPool(int id)
        {
            return new VstsRestRequest<Response.AgentPoolInfo>($"_apis/distributedtask/pools/{id}", Method.GET);
        }

        public static IVstsRestRequest<Response.Multiple<Response.AgentStatus>> AgentPoolStatus(int id)
        {
            return new VstsRestRequest<Response.Multiple<Response.AgentStatus>>($"_apis/distributedtask/pools/{id}/agents?includeCapabilities=false&includeAssignedRequest=true", Method.GET);
        }
    }
}