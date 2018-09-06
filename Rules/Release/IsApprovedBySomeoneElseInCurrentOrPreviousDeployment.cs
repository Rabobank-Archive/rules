using System;
using System.Collections.Generic;
using System.Linq;
using r = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Release
{
    public class IsApprovedBySomeoneElseInCurrentOrPreviousDeployment
    {
        private readonly Func<r.Environment, bool> approved;

        public IsApprovedBySomeoneElseInCurrentOrPreviousDeployment() :
            this(SecurePipelineScan.Rules.Checks.Environment.IsApprovedBySomeoneElse)
        {
        }

        internal IsApprovedBySomeoneElseInCurrentOrPreviousDeployment(Func<r.Environment, bool> approved)
        {
            if (approved == null)
            {
                throw new ArgumentNullException(nameof(approved));
            }

            this.approved = approved;
        }
        
        public bool GetResult(r.Release release, string environmentId)
        {
            if (release.Environments != null)
            {
                var env = release.Environments.SingleOrDefault(e => e.Id == environmentId);
                if (env != null)
                {
                    return IsApproved(env, release.Environments.ToDictionary(e => e.Name));
                }
            }

            return false;
        }

        private bool IsApproved(r.Environment env, IDictionary<string, r.Environment> all)
        {
            return (approved(env) || 
                env.Conditions != null && env.Conditions.All(c => IsApproved(all[c.Name], all)));
        }
    }
}