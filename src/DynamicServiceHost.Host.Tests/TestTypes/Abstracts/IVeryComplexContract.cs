using System.Collections.Generic;

namespace DynamicServiceHost.Host.Tests.TestTypes.Abstracts
{
    public interface IVeryComplexContract
    {
        List<SimpleDto> DoSomething(SimpleDto dto, string message);
    }
}