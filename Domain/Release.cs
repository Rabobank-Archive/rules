using System;

namespace Domain
{
    public class Release
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Tags { get; set; }
        public string Reason { get; set; }
    }
}