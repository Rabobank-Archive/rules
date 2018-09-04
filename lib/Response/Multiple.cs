using System.Collections.Generic;

namespace lib.Response
{
    public class Multiple<T>
    {
        public int Count { get; set; }
        public List<T> Value { get; set; }
    }
}

