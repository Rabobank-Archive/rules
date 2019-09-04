using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRule
    {
        string Description { get; }
        string Why { get; }
        bool IsSox { get; }
        Task<bool> EvaluateAsync(string project, string id);
    }
}