using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectReconcile
    {
        Task ReconcileAsync(string projectId);
        string[] Impact { get; }
    }
}