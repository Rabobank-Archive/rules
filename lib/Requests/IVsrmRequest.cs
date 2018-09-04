namespace lib.Requests
{
    public interface IVsrmRequest<TResponse> : IVstsRequest<TResponse>
        where TResponse: new()
    {
    }
}