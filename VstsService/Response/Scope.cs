using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class Scope
    {
        public string RefName { get; set; }
        public Guid? RepositoryId { get; set; }
        public string MatchKind { get; set; }
    }
}
