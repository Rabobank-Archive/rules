using System.Collections.Generic;

namespace lib.tests.Response
{
    class JsonCollection<T>
    {
        public int Count { get; set; }
        public List<T> Value { get; set; }
    }
}

