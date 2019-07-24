using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService.Enumerators
{
    internal class MultipleEnumerator<TResponse> : IVstsRequestEnumerator<TResponse>
    {
        public IEnumerable<TResponse> Enumerate(IFlurlRequest request)
        {
            var more = true;
            while(more)
            {
                // Need headers & result so capture task first: https://stackoverflow.com/a/53514668/129269
                var task = request.GetAsync();

                var response = task.ConfigureAwait(false).GetAwaiter().GetResult();
                var data = task.ReceiveJson<Multiple<TResponse>>().ConfigureAwait(false).GetAwaiter().GetResult();
                
                foreach (var item in data.Value)
                {
                    yield return item;
                }

                more = response.Headers.TryGetValues("x-ms-continuationtoken", out var tokens);                
                request.SetQueryParam("continuationToken", tokens?.First());
            } 
        }
    }
}