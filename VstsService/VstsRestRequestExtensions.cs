using Newtonsoft.Json.Linq;
using System;

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

            public string Uri => _request.Uri;
        }
    }
}