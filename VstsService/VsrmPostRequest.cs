namespace SecurePipelineScan.VstsService
{
    public class VsrmPostRequest<TResponse> : VsrmRequest<TResponse>, IVstsPostRequest<TResponse>
        where TResponse: new()
    {
        public object Body { get; }

        public VsrmPostRequest(string uri, object body) : base(uri)
        {
            Body = body;
        }
    }
}