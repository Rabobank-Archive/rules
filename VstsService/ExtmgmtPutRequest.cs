namespace SecurePipelineScan.VstsService
{
    public class ExtmgmtPutRequest<TResponse> : ExtmgmtRequest<TResponse>, IVstsPostRequest<TResponse>
        where TResponse: new()
    {
        public object Body { get; }

        public ExtmgmtPutRequest(string uri, object body) : base(uri)
        {
            Body = body;
        }
    }
}