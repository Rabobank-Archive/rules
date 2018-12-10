using SecurePipelineScan.Rules.Reports;

namespace Rules.Reports
{
    public class SecurityReport : EndpointReport
    {
        public string Project { get; set; }
        public bool ApplicationGroupContainsProductionEnvironmentOwner { get; set; }
        
    }
}