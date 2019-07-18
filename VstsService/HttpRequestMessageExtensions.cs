using System;
using System.Net.Http;

namespace SecurePipelineScan.VstsService
{
    public static class HttpRequestMessageExtensions
    {
        public static bool IsExtMgtRequest(this HttpRequestMessage request, string organization)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new ExtmgmtRequest<object>(string.Empty).BaseUri(organization).IsBaseOf(request.RequestUri);
        }
    }
}