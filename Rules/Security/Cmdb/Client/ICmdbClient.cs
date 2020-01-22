using System.Threading.Tasks;
using SecurePipelineScan.Rules.Security.Cmdb.Model;

namespace SecurePipelineScan.Rules.Security.Cmdb.Client

{
    public interface ICmdbClient
    {
        CmdbClientConfig Config { get; }

        Task<CiContentItem> GetCiAsync(string ciIdentifier);

        Task<AssignmentContentItem> GetAssignmentAsync(string name);

        Task UpdateDeploymentMethodAsync(string item, ConfigurationItemModel model);
    }
}