namespace SecurePipelineScan.VstsService
{
    public class VsrmPostRequest<TInput, TResponse> : VsrmRequest<TResponse>, IVstsPostRequest<TInput, TResponse>
        where TResponse: new()
    {
        public VsrmPostRequest(string uri) : base(uri)
        {
        }
    }
}