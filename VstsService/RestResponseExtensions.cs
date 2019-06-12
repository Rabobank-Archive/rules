using System.Net;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    internal static class RestResponseExtensions
    {
        public static IRestResponse<T> ThrowOnError<T>(this IRestResponse<T> response)
        {
            ThrowInternal(response);
            return response;
        }

        public static IRestResponse ThrowOnError(this IRestResponse response)
        {
            ThrowInternal(response);
            return response;
        }

        private static void ThrowInternal(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
                throw new VstsException(response, $"Maybe your PAT is incorrect? HttpStatus: {response.StatusCode }, {response.StatusDescription}, {response.ErrorMessage ?? response.Content}");

            if (!response.IsSuccessful && response.StatusCode != HttpStatusCode.NotFound)
                throw new VstsException(response, $"HttpStatus: {response.StatusCode }, {response.StatusDescription}, {response.ErrorMessage ?? response.Content}");
        }

        public static T DefaultIfNotFound<T>(this IRestResponse<T> response)
        {
            return response.StatusCode == HttpStatusCode.NotFound ? default(T) : response.Data;
        }
    }
}