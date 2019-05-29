namespace DynamicServiceHost.Host.Tests.TestTypes
{
    public class ComplexServiceMock : IComplexContract, IAssertor
    {
        private readonly InvokeCounter invokeCounter;

        public ComplexServiceMock()
        {
            invokeCounter = new InvokeCounter();
        }

        public void GetSomeDto(SimpleDto dto)
        { 
            invokeCounter.AddInvokation(nameof(GetSomeDto), new[] { dto });
        }

        void IAssertor.AssertInvokation(string methodName, object[] methodParams)
        {
            invokeCounter.AssertInvokation(methodName, methodParams);
        }
    }
}
