using System;
using RestSharp;

namespace lib
{
    public interface IVstsRequest<TResponse> : IRestRequest
        where TResponse: new()
    {
        Uri BaseUri(string organization);
    }
}