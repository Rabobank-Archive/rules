using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Common
{
    public enum PermissionId
    {
        [Display(Name = "Not Set")] NotSet = 0,
        [Display(Name = "Allow")] Allow = 1,
        [Display(Name = "Deny")] Deny = 2,
        [Display(Name = "Allow (inherited)")] AllowInherited = 3,
        [Display(Name = "Deny (inherited)")] DenyInherited = 4,
    }
}