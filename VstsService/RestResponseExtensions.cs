using RestSharp;
using System;
using System.Net;

namespace SecurePipelineScan.VstsService
{
    public static class RestResponseExtensions
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
            if (!response.IsSuccessful && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new Exception(response.ErrorMessage ?? response.Content);
            }

            if (response.StatusCode == HttpStatusCode.NonAuthoritativeInformation)
            {
                throw new Exception($"Maybe your PAT is incorrect? HttpStatus: {response.StatusCode }, {response.StatusDescription}, {response.ErrorMessage ?? response.Content}");
            }
        }
    }
}
