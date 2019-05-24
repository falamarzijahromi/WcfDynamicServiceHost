using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicServiceHost.Matcher.Tests
{
    public static class ReflectionHelper
    {
        public static bool HasAttribute(Type type, Type attributeType, IDictionary<string, object> propsValuesMapping)
        {
            bool hasAttribute;

            try
            {
                hasAttribute = MemberHasAttribute(type, attributeType, propsValuesMapping);
            }
            catch (Exception e)
            {
                return false;
            }

            return hasAttribute;
        }

        public static bool HasAttributeOnAllProperties(Type type, Type attributeType, IDictionary<string, object> propertiesValuesMapping)
        {
            var typeProps = type.GetProperties();

            return typeProps.All(prop => MemberHasAttribute(prop, attributeType, propertiesValuesMapping));
        }

        private static bool MemberHasAttribute(
            MemberInfo member, 
            Type attributeType,
            IDictionary<string, object> propertiesValuesMapping)
        {
            var attribute = member.GetCustomAttribute(attributeType);

            foreach (var propName in propertiesValuesMapping.Keys)
            {
                var attributeProp = attributeType.GetProperty(propName);

                var propValue = attributeProp.GetValue(attribute);

                if (!propValue.Equals(propertiesValuesMapping[propName]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
