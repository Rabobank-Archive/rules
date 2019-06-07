using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.VstsService
{
    public interface IRestClient
    {
        IRestResponse Execute(IRestRequest request);
        IRestResponse<T> Execute<T>(IRestRequest request) where T : new();
        IRestResponse<T> Deserialize<T>(IRestResponse response);
    }
}