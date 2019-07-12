using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using DynamicServiceHost.Matcher.Tests.TestTypes;
using Xunit;

namespace DynamicServiceHost.Matcher.Tests
{
    public class SingleModuleServicePackingTests
    {
        [Fact]
        public void All_Types_Must_Be_Packed_Within_A_Module_Builder()
        {
            var asmName = new AssemblyName("SomeName");

            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);

            var moduleBuilder = asmBuilder.DefineDynamicModule("DynamicModule");

            var testType1 = typeof(IComplexInterface);
            var testType2 = typeof(IComplexInterface);

            IGlobalTypeContainer typeContainer = new TestGlobalTypeContainer();

            var optPack = new TestOptimizationPackage() {moduleBuilder = moduleBuilder, typeContainer = typeContainer };

            var matcher1 = new ServiceMatcher(testType1, TypeCategories.Interface, optimizationPackage: optPack);
            var matcher2 = new ServiceMatcher(testType2, TypeCategories.Dto, optimizationPackage: optPack);

            var packed1 = matcher1.Pack();
            var packed2 = matcher2.Pack();
        }
    }
}
