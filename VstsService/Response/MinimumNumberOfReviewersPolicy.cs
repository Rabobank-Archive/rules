using System;
using System.Collections.Generic;
using System.Text;

namespace SecurePipelineScan.VstsService.Response
{
    public class MinimumNumberOfReviewersPolicy
    {
        public string Id { get; internal set; }

        public bool? IsEnabled { get; set; }
        public bool? IsBlocking { get; set; }
        public bool? IsDeleted { get; set; }

        public MinimumNumberOfReviewersPolicySettings Settings { get; set; }

    }

    public class MinimumNumberOfReviewersPolicySettings
    {
        public int? MinimumApproverCount { get; set; }
        public bool? CreatorVoteCounts { get; set; }
        public bool? AllowDownvotes { get; set; }
        public bool? ResetOnSourcePush { get; set; }

        public List<Scope> Scope { get; set; }

    }
}
