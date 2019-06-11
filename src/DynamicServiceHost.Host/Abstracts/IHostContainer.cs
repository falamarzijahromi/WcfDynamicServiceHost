using System;

namespace DynamicServiceHost.Host.Abstracts
{
    public interface IHostContainer : IDisposable
    {
        IHostContainer CreateLifeScope();

        void RegisterTransient(Type from, Type to);

        void RegisterSingleton<T>(T instance);

        object Resolve(Type type);
    }
}