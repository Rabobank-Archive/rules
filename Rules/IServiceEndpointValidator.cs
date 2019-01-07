using System;

namespace SecurePipelineScan.Rules
{
    public interface IServiceEndpointValidator
    {
        bool IsProductionEnvironment(string project, string releaseId, string environmentId);
        bool IsProduction(string project, Guid id);
    }
}