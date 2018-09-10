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
            this(SecurePipelineScan.Rules.Checks.Environment.IsApprovedBySomeoneElse)
        {
        }

        internal FourEyesOnAllBuildArtefacts(Func<r.Environment, bool> approved)
        {
            if (approved == null)
            {
                throw new ArgumentNullException(nameof(approved));
            }

            this.approved = approved;
        }

        public bool GetResult(r.Release release, string environmentId)
        {
            var env = release.Environments?.SingleOrDefault(e => e.Id == environmentId);
            if (env != null)
            {
                return IsApproved(env, release.Environments.ToDictionary(e => e.Name));
            }

            return false;
        }

        private bool IsApproved(r.Environment env, IDictionary<string, r.Environment> all)
        {
            return (approved(env) ||
                IsApprovedByPreviousEnvironments(env, all));
        }

        private bool IsApprovedByPreviousEnvironments(r.Environment env, IDictionary<string, r.Environment> all)
        {
            var previous = env.Conditions?.Where(c => c.ConditionType == "environmentState");
            return previous != null && 
                previous.Any() && 
                previous.All(c => IsApproved(all[c.Name], all));
        }
    }
}