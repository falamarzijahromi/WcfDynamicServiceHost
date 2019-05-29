using System;
using System.Collections.Generic;
using Xunit;

namespace DynamicServiceHost.Host.Tests.TestTypes
{
    internal class InvokeCounter
    {
        private readonly IList<Tuple<string, object[]>> invokationInformation;

        public InvokeCounter()
        {
            invokationInformation = new List<Tuple<string, object[]>>();
        }

        public void AddInvokation(string methodName, params object[] @params)
        {
            var invokationInfo = new Tuple<string, object[]>(methodName, @params);

            invokationInformation.Add(invokationInfo);
        }

        internal void AssertInvokation(string methodName, params object[] methodParams)
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
