using System.Collections.Generic;

namespace DynamicServiceHost.Host.Tests.TestTypes.Abstracts
{
    public interface ITwoMethodScenario
    {
        List<SimpleDto> GetSimpleDtos(string param);
        List<SimpleDto> GetSimpleDtos2(string param);
    }
}