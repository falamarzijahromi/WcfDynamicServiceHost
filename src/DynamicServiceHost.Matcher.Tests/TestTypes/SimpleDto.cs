using System;
using System.Collections.Generic;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public class SimpleDto
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public Guid Id { get; set; }
        public List<string> NickNames { get; set; }
        public int[] Numbers { get; set; }
    }
}
