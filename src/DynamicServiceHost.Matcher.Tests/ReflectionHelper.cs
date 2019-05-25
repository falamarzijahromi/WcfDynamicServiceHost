﻿using System;
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

        public static bool TypeHasName(Type type, string typeName)
        {
            return type.Name.Equals(typeName);
        }

        public static bool HasSamePropertiesRecursive(Type type, Type matchedType)
        {
            var typeProps = type.GetProperties();

            foreach (var typeProp in typeProps)
            {
                var matchedProp = matchedType.GetProperty(typeProp.Name);

                if (matchedProp == null || !matchedProp.PropertyType.Name.Equals(typeProp.PropertyType.Name))
                {
                    return false;
                }

                return HasSamePropertiesRecursive(typeProp.PropertyType, matchedProp.PropertyType);
            }

            return true;
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
