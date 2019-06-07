using System.Collections.Generic;
using System.Net;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.VstsService.Response
{
    public interface IRestResponse
    {
        bool IsSuccessful { get; }
        /// <summary>
        /// Transport or other non-HTTP error generated while attempting request
        /// </summary>
        string ErrorMessage { get; set; }
        /// <summary>
        /// String representation of response content
        /// </summary>
        string Content { get; set; }
        /// <summary>
        /// HTTP response status code
        /// </summary>
        HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Status of the request. Will return Error for transport errors.
        /// HTTP errors will still return ResponseStatus.Completed, check StatusCode instead
        /// </summary>
        ResponseStatus ResponseStatus { get; set; }
        /// <summary>
        /// Description of HTTP status returned
        /// </summary>
        string StatusDescription { get; set; }
        /// <summary>
        /// MIME content type of response
        /// </summary>
        string ContentType { get; set; }
        /// <summary>
        /// The RestRequest that was made to get this RestResponse
        /// </summary>
        /// <remarks>
        /// Mainly for debugging if ResponseStatus is not OK
        /// </remarks> 
        IRestRequest Request { get; set; }
        /// <summary>
        /// Headers returned by server with the response
        /// </summary>
        IList<(string key, string value)> Headers { get; }
    }
    
    public interface IRestResponse<T> : IRestResponse
    {
        /// <summary>
        /// Deserialized entity data
        /// </summary>
        T Data { get; set; }
    }
}