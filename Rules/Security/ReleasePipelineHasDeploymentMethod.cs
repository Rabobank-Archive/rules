using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Security
{
    public class ReleasePipelineHasDeploymentMethod : IReleasePipelineRule
    {
        private readonly IProductionItemsResolver _productionItemsResolver;

        public ReleasePipelineHasDeploymentMethod(IProductionItemsResolver productionItemsResolver)
        {
            _productionItemsResolver = productionItemsResolver;
        }

        [ExcludeFromCodeCoverage] public string Description => "Release pipeline has valid CMDB link";
        [ExcludeFromCodeCoverage] public string Link => "https://confluence.dev.somecompany.nl/x/PqKbD";

        public async Task<bool?> EvaluateAsync(string projectId, Response.ReleaseDefinition releasePipeline)
        {
            if (releasePipeline == null)
                throw new ArgumentNullException(nameof(releasePipeline));

            var stages = await _productionItemsResolver.ResolveAsync(projectId, releasePipeline.Id);
            return stages != null && (stages.Any(s => s == "NON-PROD" || releasePipeline.Environments.Any(e => e.Id == s)));
        }
    }
}

