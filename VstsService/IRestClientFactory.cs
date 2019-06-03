using System;
using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUri);
    }
}