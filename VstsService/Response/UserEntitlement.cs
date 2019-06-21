using System;

namespace SecurePipelineScan.VstsService.Response
{
    public class UserEntitlement
    {
        public DateTime LastAccessedDate { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid Id { get; set; }
        public User User { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}