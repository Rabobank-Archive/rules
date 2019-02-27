using System.Collections.Generic;

namespace Common
{
    public interface ICompliantValues
    {
        /// <summary>
        /// ApplicationGroupname + their collection of permissions
        /// </summary>
        IDictionary<string, IEnumerable<Permission>> GlobalPermissions { get; }
    }
}