using System.Threading.Tasks;

namespace SecurePipelineScan.Rules.Permissions
{
    internal interface IFor
    {
        Task<VstsService.Response.ApplicationGroups> IdentitiesAsync();
        Task<VstsService.Response.PermissionsSetId> PermissionSetAsync(VstsService.Response.ApplicationGroup identity);
        Task UpdateAsync(VstsService.Response.ApplicationGroup identity, VstsService.Response.PermissionsSetId permissionSet, VstsService.Response.Permission permission);
    }
}