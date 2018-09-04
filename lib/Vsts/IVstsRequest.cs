using System;
using RestSharp;

namespace lib.Vsts
{
    public interface IVstsRequest<TResponse> : IRestRequest
        where TResponse: new()
    {
        Uri BaseUri(string organization);
    }
}