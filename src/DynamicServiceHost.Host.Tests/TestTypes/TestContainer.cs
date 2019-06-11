using System;
using DynamicServiceHost.Host.Abstracts;
using Unity;
using Unity.Lifetime;

namespace DynamicServiceHost.Host.Tests.TestTypes
{
    public class TestContainer : IHostContainer
    {
        private readonly IUnityContainer container;

        public TestContainer()
        : this(new UnityContainer())
        {
        }

        public TestContainer(IUnityContainer container)
        {
            this.container = container;
        }

        public IHostContainer CreateLifeScope()
        {
            var childContainer = container.CreateChildContainer();

            var lifeScope = new TestContainer(childContainer);

            return lifeScope;
        }

        public void Dispose()
        {
            this.container.Dispose();
        }

        public void RegisterSingleton<T>(T instance)
        {
            container.RegisterInstance(instance);
        }

        public void RegisterTransient(Type from, Type to)
        {
            container.RegisterType(from, to, new ContainerControlledTransientManager());
        }

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }
    }
}
