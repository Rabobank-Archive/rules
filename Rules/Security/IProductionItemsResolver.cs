using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IProductionItemsResolver
    {
        Task<IEnumerable<string>> ResolveAsync(string projectId, string pipelineId);
    }
}