using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SecurePipelineScan.Rules
{
    public class ShouldBeDataFetcher
    {
        public ShouldBeData FetchGlobalShouldBePermissions(string project, Func<string> GetJsonData)
        {
            var shouldBePermissions = new Dictionary<string, IEnumerable<Permission>>();

            ShouldBeSettings result = GetShouldBeSettings(GetJsonData);

            var applicationGroups = result.GlobalPermissions;

            foreach (var applicationGroup in applicationGroups)
            {
                shouldBePermissions.Add($"[{project}]\\{applicationGroup.ApplicationGroupName}", CreatePermissionsPerApplicationGroup(applicationGroup));
            }
            return new ShouldBeData(project)
            {
                GlobalPermissions = shouldBePermissions,
            };
        }

        private static ShouldBeSettings GetShouldBeSettings(Func<string> GetJsonData)
        {
            try
            {
                var jsonData = GetJsonData();
                var result = JsonConvert.DeserializeObject<ShouldBeSettings>(jsonData);
                return result;
            }
            catch (Exception)
            {
                return new ShouldBeSettings();
            }
        }

        private static List<Permission> CreatePermissionsPerApplicationGroup(ApplicationGroup applicationGroup)
        {
            var applicationGroupPermissions = applicationGroup.Permissions;
            List<Permission> permissions = new List<Permission>();
             
            foreach (var groupPermission in applicationGroupPermissions)
            {
                permissions.Add(new Permission(groupPermission.PermissionBit, groupPermission.PermissionId) { DisplayName = groupPermission.DisplayName});
            }

            return permissions;
        }

        /// <summary>
        /// Returns JSon which contains Should Be-data
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string ReadFileForProject(string project)
        {
            var projectFilePath = Path.Combine("ShouldBeSettings", project + ".json");
            var defaultFilePath = Path.Combine("ShouldBeSettings", "default.json");

            var inputPath = File.Exists(projectFilePath) ? projectFilePath : defaultFilePath;

            return File.ReadAllText(Path.Combine(inputPath));
        }
    }
}