using DynamicTypeGenerator;
using DynamicTypeGenerator.Abstracts;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicServiceHost.Matcher
{
    public class ServiceMatcher
    {
        private readonly Type targetType;
        private readonly string namePostfix;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> typeAttributes;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> propAttributes;

        public ServiceMatcher(Type targetType, string namePostfix = null)
        {
            this.targetType = targetType;
            this.namePostfix = namePostfix ?? string.Empty;

            typeAttributes = new List<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>>();

            propAttributes = new List<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>>();
        }

        public void SetAttributeOnType(Type attributeType, Dictionary<Type, object> ctorParamsValuesMapping,
            Dictionary<string, object> propertiesValuesMapping)
        {
            typeAttributes.Add(new Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>(attributeType,
                ctorParamsValuesMapping, propertiesValuesMapping));
        }

        public void SetAttributeForAllMembers(Type attributeType, Dictionary<Type, object> ctorParamsValuesMapping, Dictionary<string, object> propertiesValuesMapping)
        {
            propAttributes.Add(new Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>(attributeType,
                ctorParamsValuesMapping, propertiesValuesMapping));
        }

        public ServicePack Pack()
        {
            var typeBuilder = CreateDynamicTypeBuilder();

            var retPack = new ServicePack();

            BuildMatchType(typeBuilder, retPack);

            var matchType = typeBuilder.Build();

            retPack.SetMatchType(matchType);

            retPack.AddRelatedType(matchType, targetType);

            return retPack;
        }

        private IDynamicTypeBuilder CreateDynamicTypeBuilder()
        {
            if (targetType.IsInterface)
            {
                DynamicTypeBuilderFactory.CreateClassBuilder(
                    $"{targetType.Name}{namePostfix}",
                    new Dictionary<string, Type> {{"contractedService", targetType}});
            }
            
            return DynamicTypeBuilderFactory.CreateDtoBuilder(targetType.Name);
        }

        private void BuildMatchType(IDynamicTypeBuilder typeBuilder, ServicePack retPack)
        {
            SetTypeAttribute(typeBuilder);

            SetProperties(typeBuilder, retPack);
        }

        private void SetProperties(IDynamicTypeBuilder typeBuilder, ServicePack retPack)
        {
            var typeProps = targetType.GetProperties();

            foreach (var prop in typeProps)
            {
                MapType(prop, retPack);

                if (retPack.RelatedTypes.ContainsKey(prop.PropertyType))
                {
                    var propertyBuilder = typeBuilder.SetProperty(prop.Name, retPack.RelatedTypes[prop.PropertyType]);

                    SetPropertyAttributes(propertyBuilder);

                    MapSubTypes(prop.PropertyType, retPack);
                }
            }
        }

        private void MapType(PropertyInfo prop, ServicePack retPack)
        {
            if (CheckMapPossiblity(prop, out Type typeToMap))
            {
                var propMatcher = new ServiceMatcher(typeToMap);

                var propServicePack = propMatcher.Pack();

                AddRelatedTypesToRetPack(retPack, propServicePack);
            }
        }

        private bool CheckMapPossiblity(PropertyInfo prop, out Type typeToMap)
        {
            typeToMap = PopGenericOrArray(prop.PropertyType);

            typeToMap = typeToMap ?? prop.PropertyType;

            return CheckMappingCondition(typeToMap);
        }

        private Type PopArrayBase(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (CheckMappingCondition(elementType))
                {
                    return elementType;
                }
            }

            return null;
        }

        private Type PopGenericOrArray(Type type)
        {
            var retTypeGenerinc = PopGeneric(type);

            var retTypeArray = PopArrayBase(type);

            return retTypeGenerinc ?? retTypeArray;
        }

        private Type PopGeneric(Type type)
        {
            if (type.IsGenericType)
            {
                var genType = type.GetGenericArguments()[0];

                if (CheckMappingCondition(genType))
                {
                    return genType;
                }
            }

            return null;
        }

        private bool CheckMappingCondition(Type type)
        {
            return type != null && type.IsClass && !type.IsSealed && type.BaseType != null && !type.IsGenericType && !type.IsArray;
        }

        private void MapSubTypes(Type propertyType, ServicePack retPack)
        {
            var props = propertyType.GetProperties();

            foreach (var prop in props)
            {
                if (CheckMapPossiblity(prop, out Type typeToMap))
                {
                    var propMatcher = new ServiceMatcher(typeToMap);

                    var propServicePack = propMatcher.Pack();

                    AddRelatedTypesToRetPack(retPack, propServicePack);
                }
            }
        }

        private void AddRelatedTypesToRetPack(ServicePack retPack, ServicePack propServicePack)
        {
            foreach (var relatedType in propServicePack.RelatedTypes)
            {
                retPack.AddRelatedType(relatedType.Key, relatedType.Value);
            }
        }

        private void SetPropertyAttributes(IDynamicPropertyBuilder propertyBuilder)
        {
            foreach (var attribute in propAttributes)
            {
                propertyBuilder.SetAttribute(attribute.Item1, attribute.Item2, attribute.Item3);
            }
        }

        private void SetTypeAttribute(IDynamicTypeBuilder typeBuilder)
        {
            foreach (var attribute in typeAttributes)
            {
                typeBuilder.SetAttribute(attribute.Item1, attribute.Item2, attribute.Item3);
            }
        }
    }
}
