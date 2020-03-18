using System;

namespace AzureDevOps.Compliancy.Rules
{
    public interface IProjectScan<out TReport>
    {
        TReport Execute(string project, DateTime date);
    }
}