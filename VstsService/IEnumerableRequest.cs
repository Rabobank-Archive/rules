using System.Collections.Generic;
using Flurl.Http;

namespace SecurePipelineScan.VstsService
{
    public interface IEnumerableRequest<TResponse>
    {
        IVstsRequest<TResponse> Request { get; }
        IEnumerable<TResponse> Enumerate(IFlurlRequest request);
    }
}