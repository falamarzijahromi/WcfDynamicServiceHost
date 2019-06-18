using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;
using DynamicWcfServiceHost.Proxy;
using System;
using System.Threading;
using System.Transactions;
using DynamicServiceHost.Host.Tests.TestTypes.Implementations;

namespace DynamicServiceHost.Host.Tests
{
    public abstract class ContractTestFixture
    {
        private readonly IHostContainer container;
        private readonly ITypeCacher typeCacher;
        private int arbitaryPort;
        private DynamicHost host;

        protected ContractTestFixture()
        {
            container = CreateContainer();
            typeCacher = new TypeCacherMock();
        }

        private IHostContainer CreateContainer()
        {
            var container = new TestContainer();

            RegisterRequiredTypes(container);

            return container;
        }

        protected void CreateOpenConnectedHost(Type contractType)
        {
            arbitaryPort = new Random(5000).Next(5000, 7000);

            host = new DynamicHost(contractType, container);

            host.CreateConnectedHost(arbitaryPort);

            host.Open();
        }

        protected void CreateOpenDisconnectedHost(Type contractType)
        {
            host = new DynamicHost(contractType, container);

            host.CreateDisconnectedHost();

            host.Open();
        }

        protected void AssertInvokationOn(Type contractType, string methodName, params object[] methodParams)
        {
            var service = (IAssertor)container.Resolve(contractType);

            service.AssertInvokation(methodName, methodParams);
        }

        protected object CallOnConnectedProxy(Type contractType, string methodName, params object[] @params)
        {
            object retObj = null;

            var channelFactory = new ChannelFactory(contractType);

            var channel = channelFactory.CreateConnectedChannel(typeCacher, arbitaryPort);

            var method = channel.GetType().GetMethod(methodName);

            using (var trxScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                retObj = method.Invoke(channel, @params);

                if (channel is IDisposable)
                {
                    ((IDisposable)channel).Dispose();
                }

                trxScope.Complete();
             }

            return retObj;
        }

        protected void CallOnDisconnectedProxy(Type contractType, string methodName, params object[] @params)
        {
            var channelFactory = new ChannelFactory(contractType);

            var channel = channelFactory.CreateDisconnectedChannel(typeCacher);

            var method = channel.GetType().GetMethod(methodName);

            using (var trxScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                method.Invoke(channel, @params);

                if (channel is IDisposable)
                {
                    ((IDisposable)channel).Dispose();
                }

                trxScope.Complete();
            }

            Thread.Sleep(1000);
        }

        protected abstract void RegisterRequiredTypes(TestContainer container);
    }
}
