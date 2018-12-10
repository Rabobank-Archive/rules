using System.Collections;
using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Multiple<T> : IEnumerable<T>
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
        
        public IEnumerator<T> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

