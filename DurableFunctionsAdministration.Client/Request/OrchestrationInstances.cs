using System.Collections.Generic;
using DurableFunctionsAdministration.Client.Response;

namespace DurableFunctionsAdministration.Client.Request
{
    public static class OrchestrationInstances
    {
        public static IRestRequest<IEnumerable<OrchestrationInstance>> List()
        {
            return new RestRequest<IEnumerable<OrchestrationInstance>>(
                $"runtime/webhooks/durableTask/instances", new Dictionary<string, object>
                {
                    {"showHistory", "true"},
                    { "top", "1000" }
                });
        }

        public static IRestRequest<OrchestrationInstance> Get(string instanceId)
        {
            return new RestRequest<OrchestrationInstance>(
                $"runtime/webhooks/durableTask/instances/{instanceId}", new Dictionary<string, object>
                {
                    {"showHistory", "true"}
                });
        }
    }
}