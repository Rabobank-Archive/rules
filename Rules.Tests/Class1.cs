
using RestSharp;
using SecurePipelineScan.Rules.Release;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Rules.Tests
{
    /// <summary>
    /// This is a test
    /// </summary>
    public class Class1
    {
        private const string Project = "SOx-compliant-demo";
        private readonly ITestOutputHelper output;

        public Class1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void MyTestMethod()
        {
            var organization = "somecompany";
            string token = "<pat>";
            var client = new VstsRestClient(organization, token);

            var response = client.Execute(Requests.ServiceEndpoint.Endpoints(Project));
            IReleaseRule[] rules = { new ApprovedByNotTheSameAsRequestedFor(), new ManualApproverRequired(), new LastModifiedByNotTheSameAsApprovedBy() };
            foreach (var item in response.Data.Value)
            {
                output.WriteLine($"Service endpoint found: {item.Id} with name: {item.Name}");


                foreach (var history in client.Execute(Requests.ServiceEndpoint.History(Project, item.Id)).Data.Value)
                {

                    output.WriteLine($"Releases found: {history.Data.Definition.Id} with name: {history.Data.Definition.Name}");

                    var release = client.Execute(new VstsRestRequest<SecurePipelineScan.VstsService.Response.Release>(history.Data.Owner.Links.Self.Href.AbsoluteUri, Method.GET));
                    release.ErrorMessage.ShouldBeNullOrEmpty();
                    foreach (var rule in rules)
                    {
                        output.WriteLine($"  {rule.GetType().Name} {rule.GetResult(release.Data)}");
                    }

                }
            }
            

        }

    }
}
