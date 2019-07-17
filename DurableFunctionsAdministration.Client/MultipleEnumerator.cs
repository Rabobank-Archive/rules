using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DurableFunctionsAdministration.Client.Request;
using DurableFunctionsAdministration.Client.Response;
using Flurl;
using Flurl.Http;

namespace DurableFunctionsAdministration.Client
{
    internal class MultipleEnumerator<TResponse> : IEnumerable<TResponse>
    {
        private readonly IFlurlRequest _request;

        public MultipleEnumerator(IRestRequest request, Uri baseUri, string taskHub, string code)
        {
            _request = new FlurlRequest(new Url(baseUri)
                .AppendPathSegment(request.Resource)
                .SetQueryParams(request.QueryParams)
                .SetQueryParam("taskHub", taskHub)
                .SetQueryParam("code", code));
        }

        public IEnumerator<TResponse> GetEnumerator()
        {
            var more = true;
            while(more)
            {
                // Need headers & result so capture task first: https://stackoverflow.com/a/53514668/129269
                var task = _request.GetAsync();

                var response = task.ConfigureAwait(false).GetAwaiter().GetResult();
                var data = task.ReceiveJson<TResponse[]>().ConfigureAwait(false).GetAwaiter().GetResult();
                
                foreach (var item in data)
                {
                    yield return item;
                }

                more = response.Headers.TryGetValues("x-ms-continuationtoken", out var tokens);                
                _request.SetQueryParam("continuationToken", tokens?.First());
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}