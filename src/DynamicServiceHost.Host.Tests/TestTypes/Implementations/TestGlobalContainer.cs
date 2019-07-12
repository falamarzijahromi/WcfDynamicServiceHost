using System;
using DynamicServiceHost.Matcher;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class TestGlobalContainer : IGlobalTypeContainer
    {
        public void Save(Type type)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string typeName)
        {
            throw new NotImplementedException();
        }

        public Type Get(string name)
        {
            throw new NotImplementedException();
        }
    }
}