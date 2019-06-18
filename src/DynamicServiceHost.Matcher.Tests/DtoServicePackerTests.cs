using DynamicServiceHost.Matcher.Tests.TestTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Xunit;

namespace DynamicServiceHost.Matcher.Tests
{
    public class DtoServicePackerTests
    {
        [Fact]
        public void Specified_Simple_Dto_Must_Be_Evaluated_With_Correct_Service_Pack()
        {
            var simpleDtoType = typeof(SimpleDto);
            var attributeType = typeof(SomeAttribute);

            var ctorParamsValuesMapping = new Dictionary<Type, object>
            {
                {typeof(string), "Hello"},
            };

            var propertiesValuesMapping = new Dictionary<string, object>
            {
                {nameof(SomeAttribute.Index), 5},
            };

            var matcher = new ServiceMatcher(simpleDtoType, TypeCategories.Dto);

            matcher.SetAttributeOnType(
                attributeType,
                ctorParamsValuesMapping,
                propertiesValuesMapping);

            matcher.SetAttributeForAllMembers(
                attributeType,
                ctorParamsValuesMapping,
                propertiesValuesMapping);

            var simpleDtoServicePack = matcher.Pack();

            AssertMatchedPropertyHasProperProperties(simpleDtoType, simpleDtoServicePack.MatchType);

            AssertOnHavingAllRelatedTypes(simpleDtoServicePack, simpleDtoType);

            AssertOnHavingAttributeOnType(simpleDtoServicePack.MatchType, attributeType, propertiesValuesMapping);

            AssertOnHavingAttributeOnAllProperties(simpleDtoServicePack.MatchType, attributeType, propertiesValuesMapping);
        }

        [Theory]
        [InlineData(typeof(SimpleDto))]
        [InlineData(typeof(ComplexDto))]
        [InlineData(typeof(VeryComplexDto))]
        [InlineData(typeof(SuperComplexDto))]
        public void Specified_Test_Dto_Type_Must_Be_Evaluated_With_Correct_Service_Pack(Type type)
        {
            var dtoType = type;

            var matcher = new ServiceMatcher(dtoType, TypeCategories.Dto);

            var complexDtoServicePack = matcher.Pack();

            AssertMatchedPropertyHasProperProperties(type, complexDtoServicePack.MatchType);

            AssertOnHavingAllRelatedTypes(complexDtoServicePack, dtoType);
        }

        private void AssertMatchedPropertyHasProperProperties(Type type, Type matchedType)
        {
            Assert.True(ReflectionHelper.HasSamePropertiesRecursive(type, matchedType));
        }

        private void AssertOnHavingAttributeOnAllProperties(Type matchType, Type attributeType, Dictionary<string, object> propertiesValuesMapping)
        {
            Assert.True(ReflectionHelper.HasAttributeOnAllProperties(matchType, attributeType, propertiesValuesMapping));
        }

        private void AssertOnHavingAttributeOnType(Type matchType, Type attributeType, Dictionary<string, object> propertiesValuesMapping)
        {
            Assert.True(ReflectionHelper.HasAttribute(matchType, attributeType, propertiesValuesMapping));
        }

        private void AssertOnHavingAllRelatedTypes(ServicePack servicePack, Type type)
        {
            Assert.NotNull(servicePack.MatchType);

            var groupedByType = servicePack.RelatedTypes.Where(rType => rType.Value.Equals(type));

            Assert.True(groupedByType.Any());

            var subProps = type.GetProperties();

            foreach (var subProp in subProps)
            {
                var propType = subProp.PropertyType;

                if (propType.IsClass &&
                    !propType.IsSealed &&
                    propType.BaseType != null &&
                    !propType.IsGenericType &&
                    !propType.IsArray)
                {
                    AssertOnHavingAllRelatedTypes(servicePack, propType);
                }
            }
        }

        private bool HasPropTypeInServicePack(PropertyInfo prop, ServicePack simpleDtoServicePack)
        {
            var propType = prop.PropertyType;

            if (propType.IsClass &&
                !propType.IsSealed &&
                propType.BaseType != null &&
                !propType.IsGenericType &&
                !propType.IsArray)
            {
                var propIsMapped = simpleDtoServicePack.RelatedTypes.ContainsKey(prop.PropertyType);

                var innerProps = propType.GetProperties();

                foreach (var inProp in innerProps)
                {
                    propIsMapped = propIsMapped && HasPropTypeInServicePack(inProp, simpleDtoServicePack);
                }

                return propIsMapped;
            }

            return true;
        }
    }
}
