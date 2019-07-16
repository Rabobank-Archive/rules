using System.Collections.Generic;
using DurableFunctionsAdministration.Client.Response;

namespace DurableFunctionsAdministration.Client.Request
{
    public class OrchestrationInstances
    {
        public static IRestRequest<List<OrchestrationInstance>> List()
        {
            return new RestRequest<List<OrchestrationInstance>>(
                $"runtime/webhooks/durableTask/instances", new Dictionary<string, object>
                {
                    {"showHistory", "true"},
                    { "createdTimeFrom", "2019-07-15T00:00:00Z"}
                });
        }
    }
}