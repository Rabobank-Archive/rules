namespace lib.Rules.Release
{
    public interface IReleaseRule
    {
        bool GetResult(vsts.Response.Release release);
    }
}