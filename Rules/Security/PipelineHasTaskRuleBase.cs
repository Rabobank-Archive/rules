using SecurePipelineScan.VstsService.Response;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SecurePipelineScan.Rules.Security
{
    public abstract class PipelineHasTaskRuleBase
    {
        protected abstract string TaskId { get; }

        public virtual async Task<bool> EvaluateAsync(BuildDefinition buildPipeline)
        {
            if (buildPipeline == null)
                throw new ArgumentNullException(nameof(buildPipeline));

            if (buildPipeline.Process.Phases == null) //Yaml pipeline
                return false;

            return buildPipeline.Process.Phases
                .Where(p => p.Steps != null)
                .SelectMany(p => p.Steps)
                .Any(s => s.Enabled && s.Task.Id == TaskId);
        }
    }
}