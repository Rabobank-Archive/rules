using System.Net.Http;

namespace SecurePipelineScan.VstsService.Requests
{
    public class RestRequest: IRestRequest
    {
        public RestRequest(string resource)
        {
            
        }

        public RestRequest(string requestUri, HttpMethod post)
        {
            throw new System.NotImplementedException();
        }

        public void AddOrUpdateParameter(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public IRestRequest AddHeader(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public IRestRequest AddJsonBody(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}