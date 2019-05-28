using System;
using System.ServiceModel;

namespace DynamicWcfServiceHost.Proxy
{
    internal class ChannelFactory<T> : ClientBase<T> where T : class
    {
        public static T CreateChannel(Type type)
        {
            var factory = (ChannelFactory<T>)Activator.CreateInstance(typeof(ChannelFactory<T>));

            return factory.Channel;
        }
    }
}
