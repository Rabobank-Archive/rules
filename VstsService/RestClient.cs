using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public class RestClient: IRestClient
    {
        public IRestResponse Execute(IRestRequest request)
        {
            throw new System.NotImplementedException();
        }

        public IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            throw new System.NotImplementedException();
        }

        public IRestResponse<T> Deserialize<T>(IRestResponse response)
        {
            throw new System.NotImplementedException();
        }
    }
}