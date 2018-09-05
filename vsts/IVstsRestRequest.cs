using System;
using RestSharp;

namespace vsts
{
    public interface IVstsRestRequest<TResponse> : IRestRequest
        where TResponse: new()
    {
        Uri BaseUri(string organization);
    }
}