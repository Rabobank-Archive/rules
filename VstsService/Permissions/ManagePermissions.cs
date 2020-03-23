using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurePipelineScan.VstsService.Permissions
{
    public class ManagePermissions
    {
        private readonly IFor _item;
        private Func<(int bit, string namespaceId), bool> _check = p => true;
        private IEnumerable<int> _allow = new int[0];
        private Func<Response.ApplicationGroup, bool> _filter = g => true;

        public ManagePermissions(IFor item) => _item = item;

        public async Task SetToAsync(int to)
        {
            var group = await _item
                .IdentitiesAsync()
                .ConfigureAwait(false);
            
            foreach (var identity in group.Identities.Where(_filter))
            {
                await SetAsync(identity, to).ConfigureAwait(false);
            }
        }

        public async Task<bool?> ValidateAsync()
        {
            var group = await _item
                .IdentitiesAsync()
                .ConfigureAwait(false);
            
            var permissions = await Task.WhenAll(
                group
                    .Identities
                    .Where(_filter)
                    .Select(identity => _item.PermissionSetAsync(identity)))
                .ConfigureAwait(false);
            
            return permissions
                .SelectMany(p => p.Permissions)
                .Where(p => _check((p.PermissionBit, p.NamespaceId)))
                .All(p => _allow.Contains(p.PermissionId));
        }

        public ManagePermissions For(params string[] names)
        {
            _filter = g => names.Contains(g.FriendlyDisplayName);
            return this;
        }

        public ManagePermissions Ignore(params string[] names)
        {
            _filter = g => !names.Contains(g.FriendlyDisplayName);
            return this;
        }

        public ManagePermissions Permissions(params int[] bits)
        {
            _check = p => bits.Any(bit => p.bit == bit);
            return this;
        }

        public ManagePermissions Permissions(params (int bit, string namespaceId)[] bits)
        {
            _check = p => bits.Any(x => x == p);
            return this;
        }

        public ManagePermissions Allow(params int[] bits)
        {
            _allow = bits;
            return this;
        }

        public static ManagePermissions ForBuildPipeline(IVstsRestClient client, string projectId, string itemId, string itemPath)
            => new ManagePermissions(new ForPipeline(client, projectId, SecurityNamespaceIds.Build, itemId, itemPath));

        public static ManagePermissions ForReleasePipeline(IVstsRestClient client, string projectId, string itemId, string itemPath)
            => new ManagePermissions(new ForPipeline(client, projectId, SecurityNamespaceIds.Release, itemId, itemPath));

        public static ManagePermissions ForRepository(IVstsRestClient client, string projectId, string itemId)
            => new ManagePermissions(new ForRepository(client, projectId, itemId));

        public static ManagePermissions ForTeamProject(IVstsRestClient client, string project)
            => new ManagePermissions(new ForTeamProject(client, project));

        public static ManagePermissions ForMasterBranch(IVstsRestClient client, string projectId, string itemId)
            => new ManagePermissions(new ForMasterBranch(client, projectId, itemId));

        private async Task SetAsync(Response.ApplicationGroup identity, int to)
        {
            var permissionSet = await _item
                .PermissionSetAsync(identity)
                .ConfigureAwait(false);
            var permissions = permissionSet
                .Permissions
                .Where(p => _check((p.PermissionBit, p.NamespaceId)) && !_allow.Contains(p.PermissionId));

            foreach (var permission in permissions)
            {
                permission.PermissionId = to;
                await _item
                    .UpdateAsync(identity, permissionSet, permission)
                    .ConfigureAwait(false);
            }
        }
    }
}