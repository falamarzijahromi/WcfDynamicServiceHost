using System;

namespace DynamicServiceHost.Matcher
{
    public interface IGlobalTypeContainer
    {
        void Save(Type type);
        bool Contains(string typeName);
        Type Get(string name);
    }
}