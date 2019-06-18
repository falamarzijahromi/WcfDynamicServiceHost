using System.Collections.Generic;
using DynamicWcfServiceHost.Proxy;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class TypeCacherMock : ITypeCacher
    {
        private Dictionary<object, object> mappings = new Dictionary<object, object>();

        public void Hold(object keyObject, object valueObject)
        {
            mappings[keyObject] = valueObject;
        }

        public object Get(object keyObject)
        {
            return mappings[keyObject];
        }

        public bool ContainesKey(object keyObject)
        {
            return mappings.ContainsKey(keyObject);
        }
    }
}