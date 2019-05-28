using System;
using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicWcfServiceHost.Proxy;
using Xunit;

namespace DynamicServiceHost.Host.Tests
{
    public class SimpleContractTests
    {
        private readonly IHostContainer container;
        private DynamicHost host;

        public SimpleContractTests()
        {
            container = CreateContainer();
        }

        [Fact]
        public void Simple_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(ISimpleContract);

            var methodNameToInvoke = nameof(ISimpleContract.DoSomethingString);

            CreateOpenHost(contractType);

            CallOnProxy(contractType, methodNameToInvoke);

            AssertInvokationOn(container, contractType, methodNameToInvoke);
        }

        private void CreateOpenHost(Type contractType)
        {
            host = new DynamicHost(contractType, container);
            host.Open();
        }

        private void AssertInvokationOn(IHostContainer container, Type contractType, string methodName, params object[] methodParams)
        {
            var service = (SimpleContractServiceMock) container.Resolve(contractType);

            service.AssertInvokation(methodName, methodParams);
        }

        private object CallOnProxy(Type contractType, string methodName, params object[] @params)
        {
            var channel = ChannelFactory.CreateConnectedChannel(contractType);

            var method = channel.GetType().GetMethod(methodName);

            return method.Invoke(channel, @params);
        }

        private IHostContainer CreateContainer()
        {
            var container = new TestContainer();

            RegisterRequiredTypes(container);

            return container;
        }

        private void RegisterRequiredTypes(TestContainer container)
        {
            container.RegisterSingleton<ISimpleContract>(new SimpleContractServiceMock());
        }
    }
}