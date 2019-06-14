using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Flurl;
using Flurl.Http;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    internal class MultipleEnumerator<TResponse> : IEnumerable<TResponse>
    {
        private readonly IVstsRequest _request;
        private readonly string _token;
        private readonly string _organization;

        public MultipleEnumerator(IVstsRequest request, string organization, string token)
        {
            _request = request;
            _token = token;
            _organization = organization;
        }

        public IEnumerator<TResponse> GetEnumerator()
        {
            string continuationtoken = null;
            
            while(true)
            {
                var request = new Url(_request.BaseUri(_organization))
                        .AppendPathSegment(_request.Resource)
                        .WithBasicAuth(string.Empty, _token)
                        .SetQueryParams(_request.QueryParams)
                        .SetQueryParam("continuationToken", continuationtoken);
                
                // Need headers & result so capture task first: https://stackoverflow.com/a/53514668/129269
                var task = request.GetAsync();

                var response = task.ConfigureAwait(false).GetAwaiter().GetResult();
                var data = task.ReceiveJson<Multiple<TResponse>>().ConfigureAwait(false).GetAwaiter().GetResult();
                
                foreach (var item in data.Value)
                {
                    yield return item;
                }

                continuationtoken = response.Headers.TryGetValues("x-ms-continuationtoken", out var values) ? values.FirstOrDefault() : null;
                if (continuationtoken == null)
                {
                    break;
                }
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}