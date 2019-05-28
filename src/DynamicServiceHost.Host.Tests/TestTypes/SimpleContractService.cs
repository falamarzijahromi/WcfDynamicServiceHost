using System;
using System.Collections.Generic;
using Xunit;

namespace DynamicServiceHost.Host.Tests.TestTypes
{
    public class SimpleContractServiceMock : ISimpleContract
    {
        private readonly List<Tuple<string, object[]>> invokationInformation;

        public SimpleContractServiceMock()
        {
            invokationInformation = new List<Tuple<string, object[]>>();
        }

        public void DoSomething()
        {
            var invokationInfo = new Tuple<string, object[]>(nameof(DoSomething), new object[0]);

            invokationInformation.Add(invokationInfo);
        }

        public string DoSomethingString()
        {
            var invokationInfo = new Tuple<string, object[]>(nameof(DoSomethingString), new object[0]);

            invokationInformation.Add(invokationInfo);

            return "Done";
        }

        internal void AssertInvokation(string methodName, object[] methodParams)
        {
            Assert.Single(invokationInformation);

            var invokationInfo = invokationInformation[0];

            Assert.Equal(methodName, invokationInfo.Item1);

            for (int i = 0; i < methodParams.Length; i++)
            {
                var arg = invokationInfo.Item2[i];
                var argExpected = methodParams[i];

                Assert.Equal(argExpected, arg);
            }
        }
    }
}
