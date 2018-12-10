namespace SecurePipelineScan.VstsService
{
    public class VstsPostRequest<TResponse> : VstsRestRequest<TResponse>, IVstsPostRequest<TResponse>
        where TResponse: new()
    {
        public object Body { get; }

        public VstsPostRequest(string uri, object body) : base(uri)
        {
            Body = body;
        }
    }
}