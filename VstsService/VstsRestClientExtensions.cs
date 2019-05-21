namespace SecurePipelineScan.VstsService
{
    public static class VstsRestClientExtensions
    {
        public static TResponse Get<TResponse>(this IVstsRestClient client, string url) where TResponse: new()
        {
            return client.Get(new VstsRequest<TResponse>(url));
        }
    }
}