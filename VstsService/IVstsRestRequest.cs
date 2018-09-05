using System;
using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public interface IVstsRestRequest<TResponse> : IRestRequest
        where TResponse: new()
    {
        Uri BaseUri(string organization);
    }
}