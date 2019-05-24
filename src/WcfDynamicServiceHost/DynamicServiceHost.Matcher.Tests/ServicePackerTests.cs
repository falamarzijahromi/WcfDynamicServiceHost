using DynamicServiceHost.Matcher.Tests.TestTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace DynamicServiceHost.Matcher.Tests
{
    public class ServicePackerTests
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

            var matcher = new ServiceMatcher(simpleDtoType);

            matcher.SetAttributeOnType(
                attributeType,
                ctorParamsValuesMapping,
                propertiesValuesMapping);

            matcher.SetAttributeForAllMembers(
                attributeType, 
                ctorParamsValuesMapping,
                propertiesValuesMapping);

            var simpleDtoServicePack = matcher.Pack();

            AssertOnHavingAttributeOnType(simpleDtoServicePack.MatchType, attributeType, propertiesValuesMapping);

            AssertOnHavingAttributeOnAllProperties(simpleDtoServicePack.MatchType, attributeType, propertiesValuesMapping);
        }

        private void AssertOnHavingAttributeOnAllProperties(Type matchType, Type attributeType, Dictionary<string, object> propertiesValuesMapping)
        {
            Assert.True(ReflectionHelper.HasAttributeOnAllProperties(matchType, attributeType, propertiesValuesMapping));
        }

        private void AssertOnHavingAttributeOnType(Type matchType, Type attributeType, Dictionary<string, object> propertiesValuesMapping)
        {
            Assert.True(ReflectionHelper.HasAttribute(matchType, attributeType, propertiesValuesMapping));
        }

        private void AssertOnHavingAllRelatedTypes(ServicePack simpleDtoServicePack)
        {
            var matchTypeProps = simpleDtoServicePack.MatchType.GetProperties();

            foreach (var prop in matchTypeProps)
            {
                Assert.True(HasPropTypeInServicePack(prop, simpleDtoServicePack));
            }
        }

        private bool HasPropTypeInServicePack(PropertyInfo prop, ServicePack simpleDtoServicePack)
        {
            return simpleDtoServicePack.RelatedTypes.ContainsKey(prop.PropertyType);
        }
    }
}