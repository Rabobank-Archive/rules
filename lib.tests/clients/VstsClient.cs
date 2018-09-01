namespace lib.tests.clients
{
    class VstsClient : AuthorizedClient
    {
        public VstsClient(string organization, string token) : base($"https://{organization}.visualstudio.com/", token)
        {
        }
    }
}

