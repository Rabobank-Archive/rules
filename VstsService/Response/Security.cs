using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Security
    {
        public class Identity
        {
            public string DisplayName { get; set; }
            public string TeamFoundationId { get; set; }
            public string Description { get; set; }
            public string FriendlyDisplayName { get; set; }
        }

        public class IdentityGroup
        {
            public IEnumerable<Identity> Identities { get; set; }
            public bool HasMore { get; set; }
            public int TotalIdentityCount { get; set; }
        }
    }
}