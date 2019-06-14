using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Security
{
    public interface IProjectReconcile
    {
        Task Reconcile(string project);
        string[] Impact { get; }
    }
}