using DynamicServiceHost.Matcher.Tests.TestTypes;
using System;
using System.Reflection;
using Xunit;

namespace DynamicServiceHost.Matcher.Tests
{
    public class InterfaceServicePackerTests
    {
        [Theory]
        [InlineData(typeof(ISimpleInterface))]
        [InlineData(typeof(IComplexInterface))]
        public void Specified_Interface_Must_Be_Evaluated_With_Proper_Service_Pack(Type type)
        {
            const string NamePostfix = "_Connect";

            var matcher = new ServiceMatcher(type, NamePostfix);

            var servicePack = matcher.Pack();

            AssertMatchTypeMappedCorrectly(type, servicePack, NamePostfix);
        }

        private void AssertMatchTypeMappedCorrectly(Type type, ServicePack servicePack, string namePostfix)
        {
            Assert.True(servicePack.MatchType.IsClass);
            Assert.True(ReflectionHelper.TypeHasName(servicePack.MatchType, $"{type.Name}{namePostfix}"));

            var methods = type.GetMethods();

            foreach (var method in methods)
            {
                Assert.NotNull(servicePack.MatchType.GetMethod(method.Name));

                CheckReturnTypeMap(method.ReturnType, servicePack);

                CheckMethodParamsMap(method, servicePack);
            }
        }

        private void CheckMethodParamsMap(MethodInfo method, ServicePack servicePack)
        {
            var @params = method.GetParameters();

            foreach (var param in @params)
            {
                var paramType = PopGenericOrArray(param.ParameterType);

                paramType = paramType ?? param.ParameterType;

                if (paramType.IsClass &&
                    !paramType.IsSealed &&
                    paramType.BaseType != null &&
                    !paramType.IsGenericType &&
                    !paramType.IsArray)
                {
                    Assert.True(servicePack.RelatedTypes.ContainsValue(paramType));

                    CheckTypePropertiesRecursively(paramType, servicePack);
                }
            }
        }

        private void CheckTypePropertiesRecursively(Type paramType, ServicePack servicePack)
        {
            foreach (var prop in paramType.GetProperties())
            {
                var propType = prop.PropertyType;

                if (propType.IsClass &&
                    !propType.IsSealed &&
                    propType.BaseType != null &&
                    !propType.IsGenericType &&
                    !propType.IsArray)
                {
                    Assert.True(servicePack.RelatedTypes.ContainsValue(propType));

                    CheckTypePropertiesRecursively(propType, servicePack);
                }
            }
        }

        private void CheckReturnTypeMap(Type returnType, ServicePack servicePack)
        {
            var popedReturnType = PopGenericOrArray(returnType);

            popedReturnType = popedReturnType ?? returnType;

            if (popedReturnType.IsClass &&
                !popedReturnType.IsSealed &&
                popedReturnType.BaseType != null &&
                !popedReturnType.IsGenericType &&
                !popedReturnType.IsArray)
            {
                Assert.True(servicePack.RelatedTypes.ContainsValue(returnType));

                CheckTypePropertiesRecursively(popedReturnType, servicePack);
            }
        }

        private Type PopGenericOrArray(Type returnType)
        {
            var popedGeneric = PopGeneric(returnType);

            var popedArray = PopArray(returnType);

            return popedGeneric ?? popedArray;
        }

        private Type PopArray(Type returnType)
        {
            var retType = default(Type);

            if (returnType.IsArray)
            {
                retType = returnType.GetElementType();
            }

            return retType;
        }

        private Type PopGeneric(Type returnType)
        {
            var retType = default(Type);

            if (returnType.IsGenericType)
            {
                retType = returnType.GetGenericArguments()[0];
            }

            return retType;
        }
    }
}