using DynamicServiceHost.Matcher.Tests.TestTypes;
using System;
using Xunit;

namespace DynamicServiceHost.Matcher.Tests
{
    public class GenericArgumentTests
    {
        [Fact]
        public void Generated_Type_Must_Have_The_Specified_Generic_Return_Type()
        {
            var targetType = typeof(IGenericReturn);

            var methodName1 = nameof(IGenericReturn.SimpleDtoGenericReturnType);
            var methodName2 = nameof(IGenericReturn.StringGenericReturnType);

            var matcher = new ServiceMatcher(targetType, TypeCategories.Interface);

            var servicePack = matcher.Pack();

            AssertOnMethodHasTheSpecifiedReturnType(
                type: targetType,
                methodName: methodName1,
                servicePack: servicePack);

            AssertOnMethodHasTheSpecifiedReturnType(
                type: targetType,
                methodName: methodName2,
                servicePack: servicePack);
        }

        private void AssertOnMethodHasTheSpecifiedReturnType(
            Type type,
            string methodName,
            ServicePack servicePack)
        {
            var typeMethod = type.GetMethod(methodName);

            Assert.True(servicePack.RelatedTypes.ContainsValue(typeMethod.ReturnType));
        }
    }
}