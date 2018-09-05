using System.Collections.Generic;

namespace Vsts.Response
{
    public class Release
    {
        public string Id { get; set; }
        public List<Environment> Environments { get; set; }
    }
}