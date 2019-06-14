using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectRule
    {
        string Description { get; }
        string Why { get; }
        Task<bool> Evaluate(string project);
    }
}