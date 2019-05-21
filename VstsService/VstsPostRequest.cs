namespace SecurePipelineScan.VstsService
{
    public class VstsPostRequest<TInput, TResponse> : VstsRestRequest<TResponse>, IVstsPostRequest<TInput, TResponse>
        where TResponse: new()
    {
        public VstsPostRequest(string uri) : base(uri)
        {
        }
    }
    
    public class VstsPostRequest<TResponse> : VstsRestRequest<TResponse>, IVstsPostRequest<TResponse>
        where TResponse: new()
    {
        public VstsPostRequest(string uri) : base(uri)
        {
        }
    }
}