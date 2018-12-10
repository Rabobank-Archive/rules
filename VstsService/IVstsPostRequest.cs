namespace SecurePipelineScan.VstsService
{
    public interface IVstsPostRequest<T> : IVstsRestRequest<T>
        where T: new()
    {
        object Body { get; }
    }
}