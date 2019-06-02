using System.Collections.Generic;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class VeryComplexServiceMock : IVeryComplexContract, IAssertor
    {
        private readonly InvokeCounter invokeCounter;

        public VeryComplexServiceMock()
        {
            invokeCounter = new InvokeCounter();
        }

        public void AssertInvokation(string methodName, object[] methodParams)
        {
            invokeCounter.AssertInvokation(methodName, methodParams);
        }

        public List<SimpleDto> DoSomething(SimpleDto dto, string message)
        {
            invokeCounter.AddInvokation(nameof(DoSomething), new object[] {dto, message});

            return new List<SimpleDto>(new []{new SimpleDto{Index = 34, Name = "sdf"} });
        }
    }
}
