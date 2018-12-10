using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Multiple<T>
    {
        public Multiple()
        {
        }
        
        public Multiple(params T[] value)
        {
            Value = value;
            Count = value.Length;
        }

        public int Count { get; set; }
        public IEnumerable<T> Value { get; set; }
    }
}

