using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    internal class MultipleEnumerator<TResponse> : IEnumerable<TResponse>
    {
        private readonly IRestRequest _request;
        private readonly IRestClient _client;

        public MultipleEnumerator(IRestRequest request, IRestClient client)
        {
            _request = request;
            _client = client;
        }

        public IEnumerator<TResponse> GetEnumerator()
        {
            while(true)
            {
                var response = _client.Execute<Multiple<TResponse>>(_request).ThrowOnError();
                foreach (var item in response.Data.Value)
                {
                    yield return item;
                }

                var token = response.Headers.FirstOrDefault(x => x.Name == "x-ms-continuationtoken");
                if (token == null)
                {
                    break;
                }
                
                _request.AddOrUpdateParameter("continuationToken", (string)token.Value);
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}