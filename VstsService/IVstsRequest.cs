using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRequest
    {
        Uri BaseUri(string organization);
        string Resource { get; }
        IDictionary<string, object> QueryParams { get; }
    }

    public interface IVstsRequest<TInput, TResponse> : IVstsRequest
        where TResponse : new()
    {
    }

    public interface IVstsRequest<TResponse> : IVstsRequest<TResponse, TResponse>
        where TResponse : new()
    {
    }
}