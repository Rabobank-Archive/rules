using System;

namespace SecurePipelineScan.Rules
{
    public interface IServiceEndpointValidator
    {
        bool CheckReleaseEnvironment(string project, string releaseId, string environmentId);
        bool IsProduction(string project, Guid id);
    }
}