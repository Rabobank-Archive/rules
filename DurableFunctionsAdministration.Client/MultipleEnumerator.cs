using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DurableFunctionsAdministration.Client.Request;
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

                var hasToken = response.Headers.TryGetValues("x-ms-continuation-token", out var tokens);
                if (hasToken)
                {
                    var foundToken = tokens?.First();

                    more = foundToken != null
                           && System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(foundToken)) != "null";
                    _request.WithHeader("x-ms-continuation-token", foundToken);
                }
                else
                {
                    more = false;
                }
            } 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}