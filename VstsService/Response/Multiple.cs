using System.Collections.Generic;

namespace SecurePipelineScan.VstsService.Response
{
    public class Multiple<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Value { get; set; }
    }
}

