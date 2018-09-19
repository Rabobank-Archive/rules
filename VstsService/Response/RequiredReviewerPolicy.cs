using System;
using System.Collections.Generic;
using System.Text;

namespace SecurePipelineScan.VstsService.Response
{
    public class RequiredReviewersPolicy
    {
        public string Id { get; internal set; }

        public bool? IsEnabled { get; set; }
        public bool? IsBlocking { get; set; }
        public bool? IsDeleted { get; set; }

        public RequiredReviewersPolicySettings Settings { get; set; }

    }

    public class RequiredReviewersPolicySettings
    {
        public List<Guid> RequiredReviewerIds { get; set; }

        public List<Scope> Scope { get; set; }

        public List<string> FileNamePatterns { get; set; }

    }

    public class Scope
    {
        public string RefName { get; set; }
        public Guid RepositoryId { get; set; }
        public string MatchKind { get; set; }
    }
}
