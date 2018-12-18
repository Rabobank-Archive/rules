using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class NameSpace
    {
        public static IVstsRestRequest<Response.Multiple<Response.NameSpace>> NameSpaces()
        {
            return new VstsRestRequest<Response.Multiple<Response.NameSpace>>(
                $"_apis/securitynamespaces?api-version=5.1-preview.1");
        }
    }
}