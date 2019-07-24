using System.Collections.Generic;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class SecurityNamespace
    {
        public static IEnumerableRequest<Response.SecurityNamespace> SecurityNamespaces() =>
            new VstsRequest<Response.SecurityNamespace>(
                $"_apis/securitynamespaces", new Dictionary<string, object>
                {
                    {"api-version", "5.1-preview.1"}
                }).AsEnumerable();
    }
}