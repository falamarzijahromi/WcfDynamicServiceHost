using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using DynamicServiceHost.Host.Abstracts;

namespace DynamicServiceHost.Host.WcfRequirements
{
    internal class HostInstanceProvider : IInstanceProvider
    {
        private readonly ConcurrentDictionary<object, IHostContainer> lifeScopesMapping;
        private readonly IHostContainer container;

        public HostInstanceProvider(IHostContainer container)
        {
            this.container = container;
            this.lifeScopesMapping = new ConcurrentDictionary<object, IHostContainer>();
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            var lifeScope = container.CreateLifeScope();

            var instance = lifeScope.Resolve(instanceContext.Host.Description.ServiceType);

            lifeScopesMapping.AddOrUpdate(instance, lifeScope, (obj, hostContainer) => lifeScope);

            return instance;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return GetInstance(instanceContext);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            lifeScopesMapping.TryRemove(instance, out IHostContainer lifeScope);

            lifeScope?.Dispose();
        }
    }
}
