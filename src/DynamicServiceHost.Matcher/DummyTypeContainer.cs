using System;

namespace DynamicServiceHost.Matcher
{
    internal class DummyTypeContainer : IGlobalTypeContainer
    {
        public void Save(Type type)
        {
        }

        public bool Contains(string typeName)
        {
            return false;
        }

        public Type Get(string name)
        {
            throw new NotImplementedException();
        }
    }
}