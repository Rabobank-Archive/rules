using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class RequiredReviewersPolicySettings
    {
        public List<Guid> RequiredReviewerIds { get; set; }

        public List<Scope> Scope { get; set; }

        public List<string> FileNamePatterns { get; set; }

    }
}
