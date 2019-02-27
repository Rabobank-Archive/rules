using System.Collections.Generic;
using System.Linq;

namespace Rules.Reports
{
    public class GlobalPermissions
    {
        public string ApplicationGroupName { get; set; }

        public IList<Permission> Permissions { get; set; }

        public bool IsCompliant => Permissions.All(p => p.IsCompliant);
    }
}