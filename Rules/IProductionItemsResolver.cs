using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOps.Compliancy.Rules
{
    public interface IProductionItemsResolver
    {
        Task<IEnumerable<string>> ResolveAsync(string projectId, string pipelineId);
    }
}