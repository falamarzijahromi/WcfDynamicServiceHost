using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class SimpleContractMock : ISimpleContract, IAssertor
    {
        private readonly InvokeCounter invokeCounter;

        public SimpleContractMock()
        {
            invokeCounter = new InvokeCounter();
        }

        public void DoSomething()
        {
            invokeCounter.AddInvokation(nameof(DoSomething));
        }

        public string DoSomethingString()
        {
            invokeCounter.AddInvokation(nameof(DoSomethingString));

            return "Done";
        }

        void IAssertor.AssertInvokation(string methodName, object[] methodParams)
        {
            invokeCounter.AssertInvokation(methodName, methodParams);
        }
    }
}
