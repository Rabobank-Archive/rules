using System.Collections.Generic;
using VstsService.Response;

namespace SecurePipelineScan.VstsService.Requests
{
    public static class YamlPipeline
    {
        public static IVstsRequest<YamlPipelineRequest, YamlPipelineResponse> Parse(string project, string pipelineId) =>
            new VstsRequest<YamlPipelineRequest, YamlPipelineResponse>(
                $"{project}/_apis/pipelines/{pipelineId}/runs", new Dictionary<string, object>
                {
                    {"api-version", "5.1-preview"}
                });

        public class YamlPipelineRequest
        {
            public bool PreviewRun { get; set; } = true;
        }
    }
}