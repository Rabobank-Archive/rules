using System;
using Xunit;
using Shouldly;
using System.Linq;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ServiceEndpoint : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _vsts;

        public ServiceEndpoint(TestConfig config)
        {
            _config = config;
            _vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryServiceConnections()
        {
            var endpoints = _vsts.Get(Requests.ServiceEndpoint.Endpoints(_config.Project));
            endpoints.ShouldNotBeEmpty();

            var endpoint = endpoints.First();
            endpoint.Name.ShouldNotBeNullOrEmpty();
            endpoint.Id.ShouldNotBe(Guid.Empty);
            endpoint.Type.ShouldNotBeNullOrEmpty();
            endpoint.Url.ShouldNotBeNullOrEmpty();
        }

        //[Fact]
        //public void QueryServiceConnectionHistory()
        //{
        //    //your user needs to be endpoint admin in the project you are running this.
        //    //your PAT needs "ALL scope". selecting all checkboxes does not work.
            
        //    var history = _vsts.Get(Requests.ServiceEndpoint.History(_config.Project, 
        //        _config.ServiceEndpointId));
        //    history.ShouldNotBeEmpty();
        //    history.ShouldAllBe(e => e.Data != null);
        //    history.ShouldAllBe(e => e.Data.Definition != null);
        //    history.ShouldAllBe(e => !string.IsNullOrEmpty(e.Data.Definition.Id));

        //    var data = history.First().Data;
        //    data.Id.ShouldNotBe(0);
        //    data.Owner.ShouldNotBeNull();
        //    data.Owner.Id.ShouldNotBe(0);
        //    data.Owner.Name.ShouldNotBeNullOrEmpty();
        //    data.Owner.Links.ShouldNotBeNull();
        //    data.Owner.Links.Web.ShouldNotBeNull();
        //    data.Owner.Links.Self.ShouldNotBeNull();
        //    data.Owner.Links.Self.Href.ShouldNotBeNull();
        //    data.PlanType.ShouldNotBeNullOrEmpty();
        //}
    }
}
