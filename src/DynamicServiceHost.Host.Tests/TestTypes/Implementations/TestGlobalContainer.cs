using System;
using System.Collections.Concurrent;
using DynamicServiceHost.Matcher;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class TestGlobalContainer : IGlobalTypeContainer
    {
        private readonly ConcurrentDictionary<string, Type> mapping;

        public TestGlobalContainer()
        {
            mapping = new ConcurrentDictionary<string, Type>();
        }
        public void Save(Type type) => mapping[type.FullName] = type;

        public bool Contains(string typeName) => mapping.ContainsKey(typeName);

        public Type Get(string name) => mapping[name];
    }
}