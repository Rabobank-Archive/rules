namespace lib.tests.clients
{
    class VsrmClient : AuthorizedClient
    {
        public VsrmClient(string organization, string token) : base($"https://{organization}.vsrm.visualstudio.com/", token)
        {
        }
    }
}
