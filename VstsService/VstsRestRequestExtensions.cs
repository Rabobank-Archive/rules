using Newtonsoft.Json.Linq;
using System;

namespace SecurePipelineScan.VstsService
{
    public static class VstsRestRequestExtensions
    {
        public static IVstsRestRequest<JObject> AsJson<TResponse>(
            this IVstsRestRequest<TResponse> request)
            where TResponse : new()
        {
            return new JsonRequest(request);
        }

        private class JsonRequest : IVstsRestRequest<JObject>
        {
            private readonly IVstsRestRequest _request;

            public JsonRequest(IVstsRestRequest request)
            {
                _request = request;
            }

            public Uri BaseUri(string organization) => _request.BaseUri(organization);

            public string Uri => _request.Uri;
        }
    }
}