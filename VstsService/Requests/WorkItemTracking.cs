using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class WorkItemTracking
    {
        public static VstsRequest<QueryByWiql, WorkItemQueryResult> QueryByWiql(string project) =>
            new VstsRequest<QueryByWiql, WorkItemQueryResult>(
                $"{project}/_apis/wit/wiql", new Dictionary<string, object>
                {
                    ["api-version"] = "5.1"
                });
    }
}