using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectRule : IRule
    {
        Task<bool> EvaluateAsync(string projectId);
    }
}