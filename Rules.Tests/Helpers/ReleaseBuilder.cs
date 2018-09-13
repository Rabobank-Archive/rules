using System;
using System.Collections.Generic;
using System.Linq;
using Response = SecurePipelineScan.VstsService.Response;

namespace Rules.Tests.Helpers
{
    public class ReleaseBuilder
    {
        private Response.Release release;

        public static implicit operator Response.Release(ReleaseBuilder builder)
        {
            return builder.release;
        }

        public static ReleaseBuilder Create()
        {
            return Create(Guid.NewGuid(), Guid.NewGuid());
        }

        public static ReleaseBuilder Create(Guid id, Guid modified)
        {
            return new ReleaseBuilder()
            {
                release = new Response.Release
                {
                    Environments = new List<Response.Environment>
                    {
                        new Response.Environment
                        {
                            DeploySteps = new List<Response.DeployStep>
                            {
                                new Response.DeployStep
                                {
                                    LastModifiedBy = new Response.Identity
                                    {
                                        Id = modified
                                    }
                                }
                            },
                            PreDeployApprovals = new List<Response.PreDeployApproval>
                            {
                                new Response.PreDeployApproval
                                {
                                   ApprovedBy = new Response.Identity
                                   {
                                       Id = id
                                   }
                                }
                            }
                        }
                    }
                }
            };
        }

        public ReleaseBuilder WithPreDeployApprovalStatus(string status)
        {
            release.Environments.First().
                PreDeployApprovals.First().
                Status = status;
            return this;
        }
    }
}