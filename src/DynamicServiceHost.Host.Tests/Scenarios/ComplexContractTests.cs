using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes.Implementations;
using Xunit;

namespace DynamicServiceHost.Host.Tests.Scenarios
{
    public class ComplexContractTests : ContractTestFixture
    {
        [Fact]
        public void Connected_Complex_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(IComplexContract);
            var invokationMethod = nameof(IComplexContract.GetSomeDto);
            var simpleDto = new SimpleDto {Index = 57, Name = "SomeName"};

            CreateOpenConnectedHost(contractType);

            CallOnConnectedProxy(contractType, invokationMethod, new[] { simpleDto });

            AssertInvokationOn(contractType, invokationMethod, new[] { simpleDto });
        }

        [Fact]
        public void Disconnected_Complex_Contract_Must_Be_Hosted_And_Invoked_Correctly()
        {
            var contractType = typeof(IComplexContract);
            var invokationMethod = nameof(IComplexContract.GetSomeDto);
            var simpleDto = new SimpleDto { Index = 57, Name = "SomeName" };

            CreateOpenDisconnectedHost(contractType);

            CallOnDisconnectedProxy(contractType, invokationMethod, new[] { simpleDto });

            AssertInvokationOn(contractType, invokationMethod, new[] { simpleDto });
        }

        protected override void RegisterRequiredTypes(TestContainer container)
        {
            container.RegisterSingleton<IComplexContract>(new ComplexServiceMock());
        }
    }
}