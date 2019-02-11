using RestSharp;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace SecurePipelineScan.VstsService
{
    [Serializable]
    public class VstsException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public VstsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VstsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public VstsException(IRestResponse response) : base(response.ErrorMessage ?? response.Content)
        {
            StatusCode = response.StatusCode;
        }

        public VstsException(IRestResponse response, string message) : base(message)
        {
            StatusCode = response.StatusCode;
        }
    }
}