using System;

namespace SecurePipelineScan.Rules
{
    public interface IProjectScan<out TReport>
    {
        TReport Execute(string project, DateTime date);
    }
}