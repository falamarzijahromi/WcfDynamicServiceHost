using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public class SimpleDto2
    {
        public string String { get; set; }
        public ConsoleColor Enum { get; set; }
        public object Object { get; set; }
        public Guid Guid { get; set; }
        public int Int { get; set; }

        public List<string> StringList { get; set; }
        public List<ConsoleColor> EnumList { get; set; }
        public List<object> ObjectList { get; set; }
        public List<int> IntList { get; set; }

        public int[] IntArray { get; set; }
        public Guid[] GuidArray { get; set; }
        public ConsoleColor[] EnumArray { get; set; }
        public object[] ObjectArray { get; set; }
        public string[] StringArray { get; set; }
    }
}
