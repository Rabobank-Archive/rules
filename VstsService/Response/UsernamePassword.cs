using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class UsernamePassword : IServiceEndpointAuthorization
    {
        public UsernamePassword(string username, string password)
        {
            Parameters["username"]=username;
            Parameters["password"]=password;
        }
        public IDictionary<string,object> Parameters { get; } = new Dictionary<string,object>();
        public string Scheme => "UsernamePassword";
    }
}
