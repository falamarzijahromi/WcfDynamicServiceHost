using System;
using System.Collections.Generic;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public class TestGlobalTypeContainer : IGlobalTypeContainer
    {
        private readonly Dictionary<string, Type> typeMappings = new Dictionary<string, Type>();

        public void Save(Type type)
        {
            typeMappings[type.Name] = type;
        }

        public bool Contains(string typeName)
        {
            return typeMappings.ContainsKey(typeName);
        }

        public Type Get(string name)
        {
            return typeMappings[name];
        }
    }
}