using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicWcfServiceHost.Proxy;
using System;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;

namespace DynamicServiceHost.Host.Tests
{
    public abstract class ContractTestFixture
    {
        private readonly IHostContainer container;
        private int arbitaryPort;
        private DynamicHost host;

        protected ContractTestFixture()
        {
            container = CreateContainer();
        }

        private IHostContainer CreateContainer()
        {
            var container = new TestContainer();

            RegisterRequiredTypes(container);

            return container;
        }

        protected void CreateOpenHost(Type contractType)
        {
            arbitaryPort = new Random(5000).Next(5000, 7000);

            host = new DynamicHost(contractType, container, port: arbitaryPort);
            host.Open();
        }

        protected void AssertInvokationOn(Type contractType, string methodName, params object[] methodParams)
        {
            var service = (IAssertor)container.Resolve(contractType);

            service.AssertInvokation(methodName, methodParams);
        }

        protected object CallOnProxy(Type contractType, string methodName, params object[] @params)
        {
            var channel = ChannelFactory.CreateConnectedChannel(contractType, arbitaryPort);

            var method = channel.GetType().GetMethod(methodName);

            return method.Invoke(channel, @params);
        }

        protected abstract void RegisterRequiredTypes(TestContainer container);
    }
}
