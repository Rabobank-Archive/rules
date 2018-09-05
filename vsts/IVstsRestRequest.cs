using System;
using RestSharp;

namespace Vsts
{
    public interface IVstsRestRequest<TResponse> : IRestRequest
        where TResponse: new()
    {
        Uri BaseUri(string organization);
    }
}