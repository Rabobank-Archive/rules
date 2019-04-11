using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class SecurityNamespace
    {
        public static IVstsRestRequest<Multiple<Response.SecurityNamespace>> SecurityNamespaces()
        {
            return new VstsRestRequest<Multiple<Response.SecurityNamespace>>(
                $"_apis/securitynamespaces?api-version=5.1-preview.1");
        }
    }
}