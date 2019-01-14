using System;

namespace SecurePipelineScan.Rules
{
    public interface IServiceEndpointValidator
    {
        bool IsProduction(string project, Guid id);
    }
}