using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class TaskGroup
    {
        public static IVstsRequest<Response.TaskGroupResponse> TaskGroupById(string project, string id) =>
    new VstsRequest<Response.TaskGroupResponse>($"{project}/_apis/distributedtask/taskgroups/{id}", new Dictionary<string, object>
    {
                { "api-version", "6.0-preview.1" }
    });
    }
}