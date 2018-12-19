using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class Namespace
    {
        public static IVstsRestRequest<Response.Multiple<Response.Namespace>> SecurityNamespaces()
        {
            return new VstsRestRequest<Response.Multiple<Response.Namespace>>(
                $"_apis/securitynamespaces?api-version=5.1-preview.1");
        }
    }
}