using System;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class RequiredReviewersPolicySettings
    {
        public IList<Guid> RequiredReviewerIds { get; set; }

        public IList<Scope> Scope { get; set; }

        public IList<string> FileNamePatterns { get; set; }

    }
}
