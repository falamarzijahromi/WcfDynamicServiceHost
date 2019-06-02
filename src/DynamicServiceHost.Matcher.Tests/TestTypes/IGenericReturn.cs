using System.Collections.Generic;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public interface IGenericReturn
    {
        SimpleDto[] StringGenericReturnType();
        IList<SimpleDto> SimpleDtoGenericReturnType();
    }
}