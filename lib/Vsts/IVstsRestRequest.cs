using System;
using RestSharp;

namespace lib.Vsts
{
    public interface IVstsRestRequest<TResponse> : IRestRequest
        where TResponse: new()
    {
        Uri BaseUri(string organization);
    }
}