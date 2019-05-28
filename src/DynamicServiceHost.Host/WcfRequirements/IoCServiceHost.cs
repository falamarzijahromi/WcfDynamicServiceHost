using System;
using System.ServiceModel;

namespace DynamicServiceHost.Host.WcfRequirements
{
    internal class IocServiceHost : ServiceHost
    {
        public IocServiceHost(Type type, HostInstanceProviderBehavior behavior) 
            : base(type)
        {
            foreach (var contract in ImplementedContracts.Values)
            {
                contract.Behaviors.Add(behavior);
            }
        }
    }
}
