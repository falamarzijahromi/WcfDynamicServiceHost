using System;

namespace DynamicServiceHost.Host.Abstracts
{
    public interface IHostContainer
    {
        void RegisterTransient(Type from, Type to);

        object Resolve(Type type);
    }
}