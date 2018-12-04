using System;
using System.Collections.Generic;
using System.Linq;
using r = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release
{
    public class IsStageApproved
    {
        private readonly Func<r.Environment, bool> approved;

        public IsStageApproved() :
            this(Checks.Environment.IsApprovedBySomeoneElse)
        {
        }

        internal IsStageApproved(Func<r.Environment, bool> approved)
        {
            this.approved = approved ?? throw new ArgumentNullException(nameof(approved));
        }

        public bool GetResult(r.Release release, int environmentId)
        {
            var env = release.Environments?.SingleOrDefault(e => e.Id == environmentId);
            return env != null && approved(env);
        }
    }
}