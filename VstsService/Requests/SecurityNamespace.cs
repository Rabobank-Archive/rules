using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class SecurityNamespace
    {
        public static IVstsRequest<Multiple<Response.SecurityNamespace>> SecurityNamespaces()
        {
            return new VstsRequest<Multiple<Response.SecurityNamespace>>(
                $"_apis/securitynamespaces", new Dictionary<string, string>
                {
                    {"api-version", "5.1-preview.1"}
                });
        }
    }
}