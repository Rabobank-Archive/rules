using System.Net.Http;

namespace SecurePipelineScan.VstsService.Security
{
    public static class TokenizerExtension
    {
        public static string IdentifierFromClaim(this ITokenizer tokenizer, HttpRequestMessage request)
        {
            if (request.Headers.Authorization == null)
            {
                return null;
            }

            var principal = tokenizer.Principal(request.Headers.Authorization.Parameter);
            return principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
    }
}