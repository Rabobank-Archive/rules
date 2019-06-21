using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService
{
    public static class VstsRestRequestExtensions
    {
        public static IVstsRequest<JObject> AsJson<TResponse>(
            this IVstsRequest<TResponse> request)
            where TResponse : new()
        {
            return new JsonRequest(request);
        }

        private class JsonRequest : IVstsRequest<JObject>
        {
            private readonly IVstsRequest _request;

            public JsonRequest(IVstsRequest request)
            {
                _request = request;
            }

            public Uri BaseUri(string organization) => _request.BaseUri(organization);

            public string Resource => _request.Resource;
            public IDictionary<string, object> QueryParams => _request.QueryParams;
        }
    }
}