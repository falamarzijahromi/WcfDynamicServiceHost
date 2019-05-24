using System;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public class SomeAttribute : Attribute
    {
        private readonly string name;

        public SomeAttribute(string name)
        {
            this.name = name;
        }

        public int Index { get; set; }
    }
}