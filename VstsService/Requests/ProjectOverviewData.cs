using SecurePipelineScan.VstsService.Response;
using System.Collections.Generic;
using static SecurePipelineScan.VstsService.Requests.ExtensionManagement;

namespace SecurePipelineScan.VstsService.Requests
{
    /// <summary>
    /// Use the projectname as the Id
    /// </summary>
    public class ProjectOverviewData : ExtensionData
    {
        public IEnumerable<Namespace> Namespaces { get; set; }
        
    }
}