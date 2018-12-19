using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class SecurityNamespace
    {
        public static IVstsRestRequest<Response.Multiple<Response.SecurityNamespace>> SecurityNamespaces()
        {
            return new VstsRestRequest<Response.Multiple<Response.SecurityNamespace>>(
                $"_apis/securitynamespaces?api-version=5.1-preview.1");
        }
    }
}