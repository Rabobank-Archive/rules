using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class PipelineHasTaskRuleBase
    {
        readonly IVstsRestClient _client;
        protected abstract string TaskId { get; }

        protected PipelineHasTaskRuleBase(IVstsRestClient client)
        {
            _client = client;
        }

        public virtual Task<bool> EvaluateAsync(string projectId, string buildPipelineId)
        {
            if (projectId == null)
                throw new ArgumentNullException(nameof(projectId));
            if (buildPipelineId == null)
                throw new ArgumentNullException(nameof(buildPipelineId));

            return EvaluateInternalAsync(projectId, buildPipelineId);
        }

        private async Task<bool> EvaluateInternalAsync(string projectId, string buildPipelineId)
        {
            var buildPipeline = await _client.GetAsync(VstsService.Requests.Builds.BuildDefinition(projectId, buildPipelineId))
                .ConfigureAwait(false);
            return PipelineHasTask(buildPipeline, TaskId);
        }

        private static bool PipelineHasTask(BuildDefinition buildPipeline, string taskId)
        {
            return buildPipeline.Process.Phases
                .SelectMany(p => p.Steps)
                .Any(s => s.Enabled && s.Task.Id == taskId);
        }
    }
}