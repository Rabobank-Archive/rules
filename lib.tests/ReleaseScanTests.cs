using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using lib.Response;
using NSubstitute;
using RestSharp;
using Shouldly;
using Xunit;

namespace lib.tests
{
    public class ReleaseScanTests
    {
        [Fact]
        public void IntegrationTestOnRelease()
        {
            const string id = "616";
            var vsts = VstsClientFactory.Create();

            var release = vsts.Execute<Response.Release>(new Requests.Release("TAS", id));
            release.ErrorMessage.ShouldBeNull();

            release.StatusCode.ShouldBe(HttpStatusCode.OK);
            release.Data.Id.ShouldBe(id);
            release.Data.Environments.ShouldNotBeEmpty();


            var env = release.Data.Environments.First();
            env.Id.ShouldNotBeNullOrEmpty();
            env.PreDeployApprovals.ShouldNotBeEmpty();
            env.DeploySteps.ShouldNotBeEmpty();

            var deploy = env.DeploySteps.First();
            deploy.RequestedFor.ShouldNotBeNull();
            deploy.RequestedFor.Id.ShouldNotBeNull();

            var predeploy = env.PreDeployApprovals.First();
            predeploy.Status.ShouldNotBeNullOrEmpty();
            predeploy.ApprovalType.ShouldNotBeNullOrEmpty();
            predeploy.IsAutomated.ShouldBe(false);
            predeploy.ApprovedBy.ShouldNotBeNull();
            predeploy.ApprovedBy.DisplayName.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void GivenPreDeployApprovalIsNotAutomated_WhenApprovedByIsSameAsRequestedFor_ThenResultIsFalse()
        {
            var id = Guid.NewGuid();
            Release release = NewRelease(id, id);

            //When
            var result = GetResult(release);

            //Then
            result.ShouldBe(false);
        }

        [Fact]
        public void GivenPreDeployApprovalIsNotAutomated_WhenApprovedByIsDifferentFromRequestedFor_ThenResultIsTrue()
        {
            Release release = NewRelease(Guid.NewGuid(), Guid.NewGuid());

            //When
            var result = GetResult(release);

            //Then
            result.ShouldBe(true);
        }

        private static Release NewRelease(Guid requestFor, Guid approvedBy)
        {
            //Given
            return new Response.Release
            {
                Environments = new List<Response.Environment> {
                    new Response.Environment {
                        DeploySteps = new List<DeployStep> {
                            new DeployStep {
                                RequestedFor = new Identity {
                                    Id = requestFor
                                }
                            }
                        },
                        PreDeployApprovals = new List<PreDeployApproval>{
                            new PreDeployApproval {
                                ApprovedBy = new Identity {
                                    Id = approvedBy
                                }
                            }
                        }
                    }
                }
            };
        }

        private static bool GetResult(Release release)
        {
            return release.Environments.All(e => e.DeploySteps.All(d => e.PreDeployApprovals.All(p => p.ApprovedBy.Id != d.RequestedFor.Id)));
        }
    }
}