namespace Rules.Rules.Release
{
    public interface IReleaseRule
    {
        bool GetResult(Vsts.Response.Release release);
    }
}