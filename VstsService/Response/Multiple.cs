using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SecurePipelineScan.VstsService.Response
{
    public class Multiple<T>
    {
        public T[] Value { get; set; }
    }
}

