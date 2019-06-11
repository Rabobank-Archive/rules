using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flurl;
using Flurl.Http;
using SecurePipelineScan.VstsService.Requests;
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
                var request = continuationtoken == null
                    ? new Url(_request.BaseUri(_organization))
                        .AppendPathSegment(_request.Resource)
                        .SetQueryParams(_request.QueryParams)
                        .WithBasicAuth(string.Empty, _token)
                    : new Url(_request.BaseUri(_organization))
                        .AppendPathSegment(_request.Resource)
                        .WithBasicAuth(string.Empty, _token)
                        .SetQueryParams(_request.QueryParams)
                        .SetQueryParam("continuationToken", continuationtoken);
                
                var task = request.GetAsync();

                var response = task.GetAwaiter().GetResult();
                var headers = response.Headers;
                var data = task.ReceiveJson<Multiple<TResponse>>().GetAwaiter().GetResult();
                
                foreach (var item in data.Value)
                {
                    yield return item;
                }

                continuationtoken = headers.TryGetValues("x-ms-continuationtoken", out var values) ? values.FirstOrDefault() : null;
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