using DynamicServiceHost.Matcher.Tests.TestTypes;
using System;
using System.Collections.Generic;
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

            var matcher = new ServiceMatcher(type, TypeCategories.Class, NamePostfix);

            var servicePack = matcher.Pack();

            AssertMatchTypeMappedCorrectly(type, servicePack, NamePostfix);
        }

        [Fact]
        public void Specified_Interface_Must_Be_Attributed_Correctly()
        {
            var interfaceType = typeof(ISimpleInterface);
            var attributeType = typeof(SomeAttribute);

            var ctorParams = new Dictionary<Type, object> {{typeof(string), "attributeName"}};
            var props = new Dictionary<string, object> {{"Index", 879}};

            var matcher = new ServiceMatcher(interfaceType, TypeCategories.Class);

            matcher.SetAttributeOnType(attributeType, ctorParams, props);

            var servicePack = matcher.Pack();

            AssertOnHavingAttribute(servicePack.MatchType, attributeType, props);
        }

        [Fact]
        public void Specified_Interface_Must_Be_Attributed_Correctly_For_All_Methods()
        {
            var interfaceType = typeof(ISimpleInterface);
            var attributeType = typeof(SomeAttribute);

            var ctorParams = new Dictionary<Type, object> { { typeof(string), "attributeName" } };
            var props = new Dictionary<string, object> { { "Index", 879 } };

            var matcher = new ServiceMatcher(interfaceType, TypeCategories.Class);

            matcher.SetAttributeForAllMembers(attributeType, ctorParams, props);

            var servicePack = matcher.Pack();

            AssertOnHavingAttributeOnAllMethods(servicePack.MatchType, attributeType, props);
        }

        [Fact]
        public void All_Types_Involved_In_Specified_Interface_Must_Be_Attributed_With_Specified_Attribute()
        {
            var interfaceType = typeof(IComplexInterface);
            var attributeType = typeof(SomeAttribute);

            var ctorParams = new Dictionary<Type, object> { { typeof(string), "attributeName" } };
            var props = new Dictionary<string, object> { { "Index", 879 } };

            var matcher = new ServiceMatcher(interfaceType, TypeCategories.Class);

            matcher.SetAttributeForAllInvolvedTypes(attributeType, ctorParams, props);

            var servicePack = matcher.Pack();

            AssertOnAllInvolvedTypesHasAttribute(servicePack, attributeType, props);
        }

        [Fact]
        public void All_Types_Member_Involved_In_Specified_Interface_Must_Be_Attributed_With_Specified_Attribute()
        {
            var interfaceType = typeof(IComplexInterface);
            var attributeType = typeof(SomeAttribute);

            var ctorParams = new Dictionary<Type, object> { { typeof(string), "attributeName" } };
            var props = new Dictionary<string, object> { { "Index", 879 } };

            var matcher = new ServiceMatcher(interfaceType, TypeCategories.Interface);

            matcher.SetAttributeForAllInvolvedTypeMembers(attributeType, ctorParams, props);

            var servicePack = matcher.Pack();

            AssertOnAllInvolvedTypeMembersHasAttribute(servicePack, attributeType, props);
        }

        private void AssertOnAllInvolvedTypeMembersHasAttribute(ServicePack servicePack, Type attributeType, Dictionary<string, object> props)
        {
            var relatedTypes = servicePack.RelatedTypes;

            relatedTypes.Remove(servicePack.MatchType);

            foreach (var involedType in relatedTypes.Keys)
            {
                Assert.True(ReflectionHelper.HasAttributeOnAllMembers(involedType, attributeType, props));
            }
        }

        private void AssertOnAllInvolvedTypesHasAttribute(ServicePack servicePack, Type attributeType, Dictionary<string, object> props)
        {
            var relatedTypes = servicePack.RelatedTypes;

            relatedTypes.Remove(servicePack.MatchType);

            foreach (var involedType in relatedTypes.Keys)
            {
                Assert.True(ReflectionHelper.HasAttribute(involedType, attributeType, props));
            }
        }

        private void AssertOnHavingAttributeOnAllMethods(Type matchType, Type attributeType, Dictionary<string, object> props)
        {
            Assert.True(ReflectionHelper.HasAttributeOnAllMethods(matchType, attributeType, props));
        }

        private void AssertOnHavingAttribute(Type matchType, Type attributeType, Dictionary<string, object> props)
        {
            Assert.True(ReflectionHelper.HasAttribute(matchType, attributeType, props));
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