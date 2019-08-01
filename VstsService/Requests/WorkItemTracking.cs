using SecurePipelineScan.VstsService.Response;
using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class WorkItemTracking
    {
        public static IVstsRequest<QueryByWiql, WorkItemQueryResult> QueryByWiql(string project, int? top = null) =>
            new VstsRequest<QueryByWiql, WorkItemQueryResult>(
                $"{project}/_apis/wit/wiql", new Dictionary<string, object>
                {
                    ["api-version"] = "5.1",
                    ["$top"] = top
                });

        public static IVstsRequest<WorkItem> GetWorkItem(string project, int id, IEnumerable<string> fields = null,
            DateTime? asOf = null) => new VstsRequest<WorkItem>(
                $"{project}/_apis/wit/workitems/{id}", new Dictionary<string, object>
                {
                    ["api-version"] = "5.1",
                    ["asOf"] = asOf,
                    ["fields"] = fields == null ? null : string.Join(",", fields)
                });
    }
}