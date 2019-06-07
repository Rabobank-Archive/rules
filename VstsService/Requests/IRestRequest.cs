namespace SecurePipelineScan.VstsService.Requests
{
    public interface IRestRequest
    {
        void AddOrUpdateParameter(string key, string value);
        IRestRequest AddHeader(string key, string value);
        IRestRequest AddJsonBody(object obj);
    }
}