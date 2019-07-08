using System.Collections.Generic;
using DynamicServiceHost.Host.Tests.TestTypes.Abstracts;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class TwoMethodScenario : ITwoMethodScenario
    {
        public List<SimpleDto> GetSimpleDtos(string param)
        {
            return new List<SimpleDto>
            {
                new SimpleDto(){Index = 1},
                new SimpleDto(){Index = 11},
                new SimpleDto(){Index = 111},
            };
        }

        public List<SimpleDto> GetSimpleDtos2(string param)
        {
            return new List<SimpleDto>
            {
                new SimpleDto(){Index = 2},
                new SimpleDto(){Index = 22},
                new SimpleDto(){Index = 222},
            };
        }
    }
}