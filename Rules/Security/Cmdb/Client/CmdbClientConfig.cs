namespace SecurePipelineScan.Rules.Security.Cmdb.Client
{
    public class CmdbClientConfig
    {
        public CmdbClientConfig(string apiKey, string endpoint, string organization)
        {
            this.ApiKey = apiKey;
            this.Endpoint = endpoint;
            this.Organization = organization;
        }

        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public string Organization { get; set; }
    }
}