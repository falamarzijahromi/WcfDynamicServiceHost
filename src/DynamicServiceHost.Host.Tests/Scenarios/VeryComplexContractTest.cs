using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes.Implementations;
using Xunit;

namespace DynamicServiceHost.Host.Tests.Scenarios
{
    public class VeryComplexContractTest : ContractTestFixture
    {
        [Fact]
        public void Connected_Very_Complex_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(IVeryComplexContract);
            var invokationMethod = nameof(IVeryComplexContract.DoSomething);
            var simpleDto = new SimpleDto { Index = 57, Name = "SomeName" };
            var message = "Hola! Very Complex Service.";

            CreateOpenConnectedHost(contractType);

            CallOnConnectedProxy(contractType, invokationMethod, new object[] { simpleDto, message });

            AssertInvokationOn(contractType, invokationMethod, new object[] { simpleDto, message });
        }

        [Fact]
        public void Disconnected_Very_Complex_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(IVeryComplexContract);
            var invokationMethod = nameof(IVeryComplexContract.DoSomething);
            var simpleDto = new SimpleDto { Index = 57, Name = "SomeName" };
            var message = "Hola! Very Complex Service.";

            CreateOpenDisconnectedHost(contractType);

            CallOnDisconnectedProxy(contractType, invokationMethod, new object[] { simpleDto, message });

            AssertInvokationOn(contractType, invokationMethod, new object[] { simpleDto, message });
        }

        protected override void RegisterRequiredTypes(TestContainer container)
        {
            container.RegisterSingleton<IVeryComplexContract>(new VeryComplexServiceMock());
        }
    }
}