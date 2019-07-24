using System.Collections.Generic;
using Flurl.Http;

namespace SecurePipelineScan.VstsService.Enumerators
{
    public interface IVstsRequestEnumerator<out TResponse>
    {
        IEnumerable<TResponse> Enumerate(IFlurlRequest request);
    }
}