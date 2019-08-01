using System;
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

        public static IRestRequest<IEnumerable<OrchestrationInstance>> ListByStatus(string[] runTimeStatusses)
        {
            return new RestRequest<IEnumerable<OrchestrationInstance>>(
                $"runtime/webhooks/durableTask/instances", new Dictionary<string, object>
                {
                    { "showHistory", "true" },
                    { "top", "1000" },
                    { "runTimeStatus", string.Join(",", runTimeStatusses) }
                });
        }

        public static IRestRequest<OrchestrationInstance> Get(string instanceId)
        {
            return new RestRequest<OrchestrationInstance>(
                $"runtime/webhooks/durableTask/instances/{instanceId}", new Dictionary<string, object>
                {
                    { "showHistory", "true" }
                });
        }

        public static IRestRequest<DeleteInstancesResponse> Delete(string instanceId)
        {
            return new RestRequest<DeleteInstancesResponse>($"runtime/webhooks/durableTask/instances/{instanceId}");
        }

        public static IRestRequest<DeleteInstancesResponse> DeleteMultiple(string[] runTimeStatusses, DateTime end)
        {
            return new RestRequest<DeleteInstancesResponse>(
                $"runtime/webhooks/durableTask/instances", new Dictionary<string, object>
                {
                    [ "runtimeStatus" ] = string.Join(",", runTimeStatusses),
                    [ "createdTimeFrom"] = DateTime.Now.Date.AddYears(-1),
                    [ "createdTimeTo" ] = end
                });
        }
    }
}