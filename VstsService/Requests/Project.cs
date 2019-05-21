using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Project
    {
        public static IVstsRequest<Multiple<Response.Project>> Projects()
        {
            return new VstsRequest<Multiple<Response.Project>>(
                $"_apis/projects?$top=1000&api-version=4.1-preview.2");
        }

        public static IVstsRequest<ProjectProperties> Properties(string project)
        {
            return new VstsRequest<ProjectProperties>(
                $"_apis/projects/{project}?api-version=5.1-preview.2");
        }
    }
}