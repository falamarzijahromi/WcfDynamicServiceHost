using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using DynamicServiceHost.Host.Abstracts;

namespace DynamicServiceHost.Host.WcfRequirements
{
    internal class HostInstanceProvider : IInstanceProvider
    {
        private readonly IHostContainer container;

        public HostInstanceProvider(IHostContainer container)
        {
            this.container = container;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return container.Resolve(instanceContext.Host.Description.ServiceType);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return GetInstance(instanceContext);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instance is IDisposable disposeInstance)
            {
                disposeInstance.Dispose();
            }
        }
    }
}
