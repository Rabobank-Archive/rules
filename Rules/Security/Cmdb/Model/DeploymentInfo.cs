using System.Diagnostics.CodeAnalysis;

namespace SecurePipelineScan.Rules.Security.Cmdb.Model
{
    [ExcludeFromCodeCoverage]
    public class DeploymentInfo
    {
        public string DeploymentMethod { get; set; }

        public string SupplementaryInformation { get; set; }
    }
}