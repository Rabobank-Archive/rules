using System.Threading.Tasks;
using SecurePipelineScan.VstsService;

namespace SecurePipelineScan.Rules.Permissions
{
    internal class ForPipeline : ForAll, IFor
    {
        private readonly IVstsRestClient _client;
        private readonly string _projectId;
        private readonly string _itemId;
        private readonly string _namespaceId;

        public ForPipeline(IVstsRestClient client, string projectId, string namespaceId, string itemId) : base(client, projectId)
        {
            _client = client;
            _projectId = projectId;
            _namespaceId = namespaceId;
            _itemId = itemId;
        }
        
        public Task<VstsService.Response.ApplicationGroups> IdentitiesAsync() => 
            _client.GetAsync(VstsService.Requests.ApplicationGroup.ExplicitIdentitiesPipelines(_projectId, _namespaceId, _itemId));
        
        public Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity) =>
            _client.GetAsync(VstsService.Requests.Permissions.PermissionsGroupSetIdDefinition(_projectId, _namespaceId,identity.TeamFoundationId, _itemId));
    }
}