using System.Collections.Generic;

namespace lib.Response
{
    public class Release
    {
        public string Id { get; set; }
        public List<Environment> Environments { get; set; }
    }
}