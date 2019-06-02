using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicWcfServiceHost.Proxy;
using System;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes.Implementations;
using Xunit;

namespace DynamicServiceHost.Host.Tests
{
    public class SimpleContractTests : ContractTestFixture
    {
        [Fact]
        public void Simple_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(ISimpleContract);
            var methodNameToInvoke = nameof(ISimpleContract.DoSomethingString);

            CreateOpenHost(contractType);

            CallOnProxy(contractType, methodNameToInvoke);

            AssertInvokationOn(contractType, methodNameToInvoke);
        }

        protected override void RegisterRequiredTypes(TestContainer container)
        {
            container.RegisterSingleton<ISimpleContract>(new SimpleContractMock());
        }
    }
}