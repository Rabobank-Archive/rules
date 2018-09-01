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
    public class ScannerTests
    {
        [Fact]
        public void IntegrationTestOnRelease()
        {
            var vsts = VstsClientFactory.Create();
            var release = vsts.Execute<Response.Release>(new Requests.Release("SOx-compliant-demo", "58"));

            release.StatusCode.ShouldBe(HttpStatusCode.OK);
            release.Data.Id.ShouldBe("58");
            release.Data.Environments.ShouldNotBeEmpty();
            
            
            var env = release.Data.Environments;
            env.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));

            var predeploy = env.SelectMany(e => e.PreDeployApprovals);
            predeploy.ShouldNotBeEmpty();

            predeploy.ShouldAllBe(p => 
                !string.IsNullOrEmpty(p.Status) &&
                !string.IsNullOrEmpty(p.ApprovalType));
        }
    }
}