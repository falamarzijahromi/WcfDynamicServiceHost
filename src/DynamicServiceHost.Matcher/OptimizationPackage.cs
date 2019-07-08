using System.Reflection.Emit;

namespace DynamicServiceHost.Matcher
{
    public interface IOptimizationPackage
    {
        ModuleBuilder moduleBuilder { get; }
        IGlobalTypeContainer typeContainer { get; }
    }
}