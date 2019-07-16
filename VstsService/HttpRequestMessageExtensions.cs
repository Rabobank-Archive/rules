using System.Net.Http;

namespace SecurePipelineScan.VstsService
{
    public static class HttpRequestMessageExtensions
    {
        public static bool IsExtMgtRequest(this HttpRequestMessage request, string organization) =>
            new ExtmgmtRequest<object>(string.Empty).BaseUri(organization).IsBaseOf(request?.RequestUri);
    }
}