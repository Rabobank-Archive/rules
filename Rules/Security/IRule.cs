using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IRule
    {
        string Description { get; }
        string Why { get; }
        Task<bool> Evaluate(string project, string id);
    }
}