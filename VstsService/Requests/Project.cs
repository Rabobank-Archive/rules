namespace SecurePipelineScan.VstsService.Requests
{
    public static class Project
    {
        public static IVstsRestRequest<Response.Multiple<Response.Project>> Projects()
        {
            return new VstsRestRequest<Response.Multiple<Response.Project>>(
                $"_apis/projects?$top=1000&api-version=4.1-preview.2");
        }
    }
}