namespace SecurePipelineScan.Rules.Release
{
    public interface IReleaseRule
    {
        bool GetResult(SecurePipelineScan.VstsService.Response.Release release);
    }
}