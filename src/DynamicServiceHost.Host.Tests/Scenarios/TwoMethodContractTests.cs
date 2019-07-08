using DynamicServiceHost.Host.Tests.TestTypes;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;
using DynamicServiceHost.Host.Tests.TestTypes.Implementations;
using Xunit;

namespace DynamicServiceHost.Host.Tests.Scenarios
{
    public class TwoMethodContractTests : ContractTestFixture
    {
        [Fact]
        public void Calling_Two_Method_Must_Return_Proper_Results()
        {
            var contractType = typeof(ITwoMethodScenario);
            var method1NameToInvoke = nameof(ITwoMethodScenario.GetSimpleDtos);
            var method2NameToInvoke = nameof(ITwoMethodScenario.GetSimpleDtos2);

            CreateOpenConnectedHost(contractType);

            CallOnConnectedProxy(contractType, method1NameToInvoke, "");
            CallOnConnectedProxy(contractType, method2NameToInvoke, "");
        }

        protected override void RegisterRequiredTypes(TestContainer container)
        {
            container.RegisterSingleton<ITwoMethodScenario>(new TwoMethodScenario());
        }
    }
}