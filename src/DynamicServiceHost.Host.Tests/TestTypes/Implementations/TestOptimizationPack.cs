using System;
using System.Reflection;
using System.Reflection.Emit;
using DynamicServiceHost.Matcher;

namespace DynamicServiceHost.Host.Tests.TestTypes.Implementations
{
    public class TestOptimizationPack : IOptimizationPackage
    {
        public TestOptimizationPack()
        {
            moduleBuilder = CreateModuleBuilder();

            typeContainer = new TestGlobalContainer();
        }

        private ModuleBuilder CreateModuleBuilder()
        {
            var asmName = new AssemblyName("ForDynamics");

            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            return asmBuilder.DefineDynamicModule("ForDynamics");
        }

        public ModuleBuilder moduleBuilder { get; }
        public IGlobalTypeContainer typeContainer { get; }
    }
}