using System;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public interface ISimpleInterface
    {
        void DoSomething(int number, string name);
        Guid DoSomething2(int number);
    }
}