using System;
using System.Collections.Generic;
using System.Linq;
using r = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release
{
    public class FourEyesOnAllBuildArtefacts
    {
        private readonly Func<r.Environment, bool> approved;

        public FourEyesOnAllBuildArtefacts() :
            this(Checks.Environment.IsApprovedBySomeoneElse)
        {
        }

        internal FourEyesOnAllBuildArtefacts(Func<r.Environment, bool> approved)
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