using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService.Permissions
{
    public interface IFor
    {
        Task<Response.ApplicationGroups> IdentitiesAsync();
        Task<Response.PermissionsSetId> PermissionSetAsync(Response.ApplicationGroup identity);
        Task UpdateAsync(Response.ApplicationGroup identity, Response.PermissionsSetId permissionSet, Response.Permission permission);
    }
}