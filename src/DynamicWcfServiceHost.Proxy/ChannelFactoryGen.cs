using System;
using System.ServiceModel;

namespace DynamicWcfServiceHost.Proxy
{
    internal class ChannelFactory<T> : ClientBase<T> where T : class
    {
        public static T CreateChannel(Type type, int? newPort = null)
        {
            var proxy = (ChannelFactory<T>)Activator.CreateInstance(typeof(ChannelFactory<T>));

            var channel = proxy.Channel;

            if (newPort.HasValue)
            {
                channel = CreateModifiedChannel(newPort, proxy, channel);
            }

            return channel;
        }

        private static T CreateModifiedChannel(int? newPort, ChannelFactory<T> proxy, T channel)
        {
            ModifyEndpointAddress(newPort, proxy);

            ((IDisposable)channel).Dispose();

            channel = proxy.ChannelFactory.CreateChannel(proxy.Endpoint.Address);

            return channel;
        }

        private static void ModifyEndpointAddress(int? newPort, ChannelFactory<T> proxy)
        {
            var endpointAddress = proxy.Endpoint.Address;

            var modifiedUri = CreateModifiedUri(endpointAddress.Uri, newPort.Value);

            endpointAddress = new EndpointAddress(modifiedUri, endpointAddress.Identity);

            proxy.Endpoint.Address = endpointAddress;
        }

        private static Uri CreateModifiedUri(Uri oldUri, int newPort)
        {
            var modifiedUriAddress = ModifyUriAddress(oldUri.ToString(), oldUri.Port, newPort);

            var newUri = new Uri(modifiedUriAddress);

            return newUri;
        }

        private static string ModifyUriAddress(string oldUriAddress, int oldPort, int newPort)
        {
            return oldUriAddress.Replace($":{oldPort}/", $":{newPort}/");
        }
    }
}
