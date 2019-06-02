using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes.Implementations;
using Xunit;

namespace DynamicServiceHost.Host.Tests.Scenarios
{
    public class SimpleContractTests : ContractTestFixture
    {
        [Fact]
        public void Connected_Simple_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(ISimpleContract);
            var methodNameToInvoke = nameof(ISimpleContract.DoSomethingString);

            CreateOpenConnectedHost(contractType);

            CallOnConnectedProxy(contractType, methodNameToInvoke);

            AssertInvokationOn(contractType, methodNameToInvoke);
        }

        [Fact]
        public void Disconnected_Simple_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(ISimpleContract);
            var methodNameToInvoke = nameof(ISimpleContract.DoSomethingString);

            CreateOpenDisconnectedHost(contractType);

            CallOnDisconnectedProxy(contractType, methodNameToInvoke);

            AssertInvokationOn(contractType, methodNameToInvoke);
        }

        protected override void RegisterRequiredTypes(TestContainer container)
        {
            container.RegisterSingleton<ISimpleContract>(new SimpleContractMock());
        }
    }
}