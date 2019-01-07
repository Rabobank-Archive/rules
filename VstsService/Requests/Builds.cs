using System.Diagnostics;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public class Builds
    {
        public static IVstsRestRequest<Response.Multiple<BuildDefinition>> BuildDefinitions(string projectId)
        {
            return new VstsRestRequest<Response.Multiple<BuildDefinition>>(
                $"{projectId}/_apis/build/definitions?api-version=5.0-preview.7");
        }
    }
}