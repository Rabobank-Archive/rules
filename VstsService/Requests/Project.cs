using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Project
    {
        public static IVstsRequest<Response.Project> ProjectById(string projectId) =>
            new VstsRequest<Response.Project>($"_apis/projects/{projectId}");

        public static IEnumerableRequest<Response.Project> Projects() =>
            new VstsRequest<Response.Project>(
                $"_apis/projects", new Dictionary<string, object>
                {
                    {"$top", "1000"},
                    {"api-version", "4.1-preview.2"}
                }).AsEnumerable();

        public static IVstsRequest<ProjectProperties> Properties(string project) =>
            new VstsRequest<ProjectProperties>(
                $"_apis/projects/{project}", new Dictionary<string, object>
                {
                    {"api-version", "5.1-preview.2"}
                });

        public static IVstsRequest<Response.Project> ProjectByName(string projectName) =>
            new VstsRequest<Response.Project>(
                $"_apis/projects/{projectName}", new Dictionary<string, object>
                {
                    {"api-version", "5.0"}
                });
    }
}