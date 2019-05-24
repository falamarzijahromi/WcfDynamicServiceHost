using DynamicTypeGenerator;
using DynamicTypeGenerator.Abstracts;
using System;
using System.Collections.Generic;

namespace DynamicServiceHost.Matcher
{
    public class ServiceMatcher
    {
        private readonly Type simpleDtoType;

        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> typeAttributes;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> propAttributes;

        public ServiceMatcher(Type simpleDtoType)
        {
            this.simpleDtoType = simpleDtoType;

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
            var typeBuilder = DynamicTypeBuilderFactory.CreateDtoBuilder(simpleDtoType.Name);

            BuildMatchType(typeBuilder);

            var matchType = typeBuilder.Build();

            var retPack = new ServicePack(matchType, new Dictionary<Type, Type> { { simpleDtoType, matchType } });

            return retPack;
        }

        private void BuildMatchType(IDynamicTypeBuilder typeBuilder)
        {
            SetTypeAttribute(typeBuilder);

            SetProperties(typeBuilder);
        }

        private void SetProperties(IDynamicTypeBuilder typeBuilder)
        {
            var typeProps = simpleDtoType.GetProperties();

            foreach (var prop in typeProps)
            {
                var propertyBuilder = typeBuilder.SetProperty(prop.Name, prop.PropertyType);

                SetPropertyAttributes(propertyBuilder);
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
