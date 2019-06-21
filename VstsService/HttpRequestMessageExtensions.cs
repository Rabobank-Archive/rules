using System;
using System.Net.Http;

namespace SecurePipelineScan.VstsService
{
    public static class HttpRequestMessageExtensions
    {
        public static bool IsExtMgtRequest(this HttpRequestMessage request, string organization)
        {
            return request.RequestUri.ToString()
                .StartsWith(new ExtmgmtRequest<Object>(string.Empty).BaseUri(organization).ToString());
        }
    }
}