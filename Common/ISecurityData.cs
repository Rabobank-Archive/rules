using System.Collections.Generic;

namespace Common
{
    public interface ISecurityData
    {
        string ProjectName { get; }

        /// <summary>
        /// Devops Group and their permissions
        /// </summary>
        IDictionary<string, IEnumerable<Permission>> GlobalPermissions { get; }
    }


}