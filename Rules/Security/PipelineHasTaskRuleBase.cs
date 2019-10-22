using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class PipelineHasTaskRuleBase
    {
        protected abstract string TaskId { get; }
        private const string MavenTaskId = "ac4ee482-65da-4485-a532-7b085873e532";

        public virtual Task<bool?> EvaluateAsync(BuildDefinition buildPipeline)
        {
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            bool? result = null;
            if (buildPipeline.Process.Type == 1)
            {
                // process.type 1 is a gui pipeline, 2 is a yaml pipeline
                if (buildPipeline.Process.Phases == null)
                    throw new ArgumentOutOfRangeException(nameof(buildPipeline));

                result = DoesPipelineContainTask(buildPipeline, TaskId);

                if (!result.GetValueOrDefault() && DoesPipelineContainTask(buildPipeline, MavenTaskId))
                    result = null;
            }

            return Task.FromResult(result);
        }

        private static bool DoesPipelineContainTask(BuildDefinition buildPipeline, string taskId) => 
            buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .Any(s => s.Enabled && s.Task.Id == taskId);
    }
}