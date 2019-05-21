using System;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRequest<TInput, TResponse> : IVstsRequest
        where TResponse: new()
    {
    }

    public interface IVstsRequest
    {
        Uri BaseUri(string organization);
        string Uri { get; }
    }

    public interface IVstsRequest<TResponse> : IVstsRequest<TResponse, TResponse>
        where TResponse : new()
    {
    }
}