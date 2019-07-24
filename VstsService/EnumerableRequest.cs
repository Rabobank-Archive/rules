using System.Collections.Generic;
using Flurl.Http;
using SecurePipelineScan.VstsService.Enumerators;

namespace SecurePipelineScan.VstsService
{
    public class EnumerableRequest<TResponse, TEnumerator> : IEnumerableRequest<TResponse> 
        where TEnumerator: IVstsRequestEnumerator<TResponse>, new()
    {
        public IVstsRequest<TResponse> Request { get; }

        public EnumerableRequest(IVstsRequest<TResponse> request)
        {
            Request = request;
        }
        
        public IEnumerable<TResponse> Enumerate(IFlurlRequest request) => 
            new TEnumerator().Enumerate(request);
    }
}