using Rules.Reports;

namespace SecurePipelineScan.Rules
{
    public interface IProjectScan<out TReport>
    {
        TReport Execute(string project);
    }
}