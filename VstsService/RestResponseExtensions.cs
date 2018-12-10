using System;
using RestSharp;

namespace SecurePipelineScan.VstsService
{
    public static class RestResponseExtensions
    {
        public static IRestResponse<T> ThrowOnError<T>(this IRestResponse<T> response)
        {
            if (!response.IsSuccessful)
            {
                throw new Exception(response.ErrorMessage ?? response.Content);
            }

            return response;
        }
    }
}