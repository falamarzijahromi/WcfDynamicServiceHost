using System;

namespace DynamicWcfServiceHost.Proxy
{
    public interface ITypeCacher
    {
        void Hold(object keyObject, object valueObject);

        object Get(object keyObject);

        bool ContainesKey(object keyObject);
    }
}