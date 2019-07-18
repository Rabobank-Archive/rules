using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IReconcile
    {
        Task ReconcileAsync(string projectId, string id);
        
        string[] Impact { get; }
    }
}