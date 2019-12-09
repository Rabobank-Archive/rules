using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Response;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineHasDeploymentMethod : IReleasePipelineRule, IReconcile
    {
        [ExcludeFromCodeCoverage] public string Description => "Release pipeline has valid CMDB link";
        [ExcludeFromCodeCoverage] public string Link => "https://confluence.dev.somecompany.nl/x/PqKbD";
        [ExcludeFromCodeCoverage] public bool IsSox => false;
        [ExcludeFromCodeCoverage] public bool RequiresStageId => true;
        [ExcludeFromCodeCoverage] public string[] Impact => new string[0];

        public Task<bool?> EvaluateAsync(string projectId, string stageId, ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            var stageExists = stageId == null
                ? (bool?)null
                : releasePipeline.Environments.Any(e => e.Id == stageId);

            return Task.FromResult(stageExists);
        }

        [ExcludeFromCodeCoverage]
        public System.Threading.Tasks.Task ReconcileAsync(string projectId, string itemId, string stageId)
        {
            // for now the reconcile happens in frontend
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }
    }
}