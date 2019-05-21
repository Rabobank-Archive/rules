namespace SecurePipelineScan.VstsService
{
    public interface IVstsPostRequest<TInput, TResponse> : IVstsRestRequest<TResponse>
        where TResponse: new()
    {
    }

    public interface IVstsPostRequest<TResponse> : IVstsPostRequest<TResponse, TResponse> where TResponse : new()
    {
    }
}