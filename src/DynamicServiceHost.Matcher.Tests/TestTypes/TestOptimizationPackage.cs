using System.Reflection.Emit;

namespace DynamicServiceHost.Matcher.Tests.TestTypes
{
    public class TestOptimizationPackage : IOptimizationPackage
    {
        public ModuleBuilder moduleBuilder { get; set; }
        public IGlobalTypeContainer typeContainer { get; set; }
    }
}