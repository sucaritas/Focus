using System;
using System.Collections.Generic;
using System.Text;

namespace FocusUsage
{
    public class Sample1
    {
        public string StringProperty { get; set; }
        public string[] StringArrayProperty { get; set; }
        public int IntProperty { get; set; }
        public int[] IntArrayProperty { get; set; }
        public Sample1 NestedProperty { get; set; }
    }
}
