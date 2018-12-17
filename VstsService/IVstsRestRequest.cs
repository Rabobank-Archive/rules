using System;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestRequest
    {
        Uri BaseUri(string organization);
        string Uri { get; }
    }
    
    public interface IVstsRestRequest<TResponse> : IVstsRestRequest
        where TResponse: new()
    {}
}