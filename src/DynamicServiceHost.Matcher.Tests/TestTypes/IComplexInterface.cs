using System;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public interface IComplexInterface
    {
        void DoSomething(int number, string name);

        Guid DoSomething2(int number);

        Guid DoSomething3(SimpleDto dto);

        SimpleDto DoSomething4(Guid id);

        SimpleDto DoSomething5(SimpleDto dto);
    }
}