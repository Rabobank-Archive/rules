using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules
{
    public interface IServiceEndpointValidator
    {
        Task<bool> ScanForProductionEndpointsAsync(string project, Guid id);
    }
}