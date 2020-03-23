using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRepositoryRule : IRule
    {
        Task<bool?> EvaluateAsync(string projectId, string repositoryId);
    }
}