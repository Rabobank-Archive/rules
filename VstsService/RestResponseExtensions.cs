using System;
using System.Net;
using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public static class RestResponseExtensions
    {
        public static IRestResponse<T> ThrowOnError<T>(this IRestResponse<T> response)
        {
            if (!response.IsSuccessful && response.StatusCode != HttpStatusCode.NotFound)
            {
                throw new Exception(response.ErrorMessage ?? response.Content);
            }

            return response;
        }

        public static IRestResponse ThrowOnError(this IRestResponse response)
        {
            if (!response.IsSuccessful)
            {
                throw new Exception(response.ErrorMessage ?? response.Content);
            }

            return response;
        }
    }
}