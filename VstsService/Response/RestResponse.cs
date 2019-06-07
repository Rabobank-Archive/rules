using System.Collections.Generic;
using System.Net;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.VstsService.Response
{
    public class RestResponse<T>: IRestResponse<T>
    {
        public bool IsSuccessful { get; }
        public string ErrorMessage { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
        public string StatusDescription { get; set; }
        public string ContentType { get; set; }
        public IRestRequest Request { get; set; }
        public IList<(string key, string value)> Headers { get; }
        public T Data { get; set; }
    }
}