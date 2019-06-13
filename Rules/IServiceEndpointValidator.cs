using System;
using System.Threading.Tasks;

namespace SecurePipelineScan.Rules
{
    public interface IServiceEndpointValidator
    {
        Task<bool> IsProduction(string project, Guid id);
    }
}