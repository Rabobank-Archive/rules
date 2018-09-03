namespace lib.Rules.Release
{
    public interface IReleaseRule
    {
        bool GetResult(Response.Release release);
    }
}