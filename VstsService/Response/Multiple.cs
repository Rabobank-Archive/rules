using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Response
{
    [JsonObject]
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
        public T[] Value { get; set; }
        
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

