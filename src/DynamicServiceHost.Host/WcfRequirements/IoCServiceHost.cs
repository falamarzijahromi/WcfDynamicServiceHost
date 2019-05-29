using System;
using System.Linq;
using System.ServiceModel;

namespace DynamicServiceHost.Host.WcfRequirements
{
    internal class IocServiceHost : ServiceHost
    {
        public IocServiceHost(Type type, HostInstanceProviderBehavior behavior, int? port) 
            : base(type)
        {
            foreach (var contract in ImplementedContracts.Values)
            {
                contract.Behaviors.Add(behavior);
            }

            if (port.HasValue)
            {
                ModifyEnpointPort(port.Value);
            }
        }

        private void ModifyEnpointPort(int newPort)
        {
            var endpoint = Description.Endpoints.Single();

            Description.Endpoints.Clear();

            ModifyEndpointPort(newPort, endpoint);

            Description.Endpoints.Add(endpoint);
        }

        private static void ModifyEndpointPort(int newPort, System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
            var endpointUri = endpoint.Address.Uri;

            var modifiedAddress = GetModifiedUri(newPort, endpointUri);

            var newUri = new Uri(modifiedAddress);

            endpoint.Address = new EndpointAddress(newUri, endpoint.Address.Identity);
        }

        private static string GetModifiedUri(int newPort, Uri endpointUri)
        {
            return endpointUri.ToString().Replace($":{endpointUri.Port}/", $":{newPort}/");
        }
    }
}
