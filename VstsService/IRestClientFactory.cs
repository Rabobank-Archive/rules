using System;

namespace SecurePipelineScan.VstsService
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUri);
    }
}