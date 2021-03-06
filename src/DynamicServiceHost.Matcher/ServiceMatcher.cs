﻿using DynamicTypeGenerator;
using DynamicTypeGenerator.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicServiceHost.Matcher
{
    public class ServiceMatcher
    {
        private readonly Type targetType;
        private readonly TypeCategories typeCategory;
        private readonly string namePostfix;
        private readonly IDictionary<string, Type> ctorExtraParamsType;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> typeAttributes;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> allMembersAttributes;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> involvedAttributes;
        private readonly IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> involvedTypeMembersAttributes;
        private readonly Type[] interfaces;
        private readonly bool allMethodsVoid;
        private readonly IOptimizationPackage optimizationPackage;
        private readonly ModuleBuilder moduleBuilder;
        private readonly IGlobalTypeContainer typeContainer;

        public ServiceMatcher(
            Type targetType, TypeCategories typeCategory,
            string namePostfix = null,
            IDictionary<string, Type> ctorExtraParamsType = null,
            bool allMethodsVoid = false,
            IOptimizationPackage optimizationPackage = null,
            params Type[] interfaces)
        {
            this.allMethodsVoid = allMethodsVoid;
            this.optimizationPackage = optimizationPackage;
            moduleBuilder = optimizationPackage?.moduleBuilder ?? CreateModuleBuilder();
            typeContainer = optimizationPackage?.typeContainer ?? new DummyTypeContainer();
            this.targetType = targetType;
            this.typeCategory = typeCategory;
            this.namePostfix = namePostfix ?? string.Empty;
            this.ctorExtraParamsType = ctorExtraParamsType ?? new Dictionary<string, Type>();
            this.interfaces = interfaces;

            typeAttributes = new List<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>>();
            allMembersAttributes = new List<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>>();

            involvedAttributes = new List<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>>();
            involvedTypeMembersAttributes = new List<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>>();
        }

        public void SetAttributeOnType(Type attributeType, IDictionary<Type, object> ctorParamsValuesMapping,
            IDictionary<string, object> propertiesValuesMapping)
        {
            typeAttributes.Add(new Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>(attributeType,
                ctorParamsValuesMapping, propertiesValuesMapping));
        }

        public void SetAttributeForAllMembers(Type attributeType, IDictionary<Type, object> ctorParamsValuesMapping, IDictionary<string, object> propertiesValuesMapping)
        {
            allMembersAttributes.Add(new Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>(attributeType,
                ctorParamsValuesMapping, propertiesValuesMapping));
        }

        public void SetAttributeForAllInvolvedTypes(Type attributeType, IDictionary<Type, object> ctorParams, IDictionary<string, object> props)
        {
            involvedAttributes.Add(new Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>(attributeType,
                ctorParams, props));
        }

        public void SetAttributeForAllInvolvedTypeMembers(Type attributeType, IDictionary<Type, object> ctorParams, IDictionary<string, object> props)
        {
            involvedTypeMembersAttributes.Add(new Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>(attributeType,
                ctorParams, props));
        }

        public ServicePack Pack()
        {
            var retPack = new ServicePack();

            if (!TryToGetFromGlobalContainer(targetType, out Type matchType))
            {
                matchType = CreateMatchType(retPack);
            }

            retPack.SetMatchType(matchType);

            retPack.AddRelatedType(matchType, targetType);

            return retPack;
        }

        private Type CreateMatchType(ServicePack retPack)
        {
            var typeBuilder = CreateDynamicTypeBuilder();

            BuildMatchType(typeBuilder, retPack);

            var matchType = typeBuilder.Build();

            typeContainer.Save(matchType);

            return matchType;
        }

        private ModuleBuilder CreateModuleBuilder()
        {
            var asmn = new AssemblyName("DynamicAssembly");

            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmn, AssemblyBuilderAccess.RunAndCollect);

            return asmBuilder.DefineDynamicModule("DynaimModule");
        }

        private IDynamicTypeBuilder CreateDynamicTypeBuilder()
        {
            var ctorParams = new Dictionary<string, Type>(ctorExtraParamsType)
            {
                {"contractedService", targetType}
            };

            switch (typeCategory)
            {
                case TypeCategories.Class:
                    return DynamicTypeBuilderFactory.CreateClassBuilder($"{targetType.Name}{namePostfix}", ctorParams, moduleBuilder);

                case TypeCategories.Dto:
                    return DynamicTypeBuilderFactory.CreateDtoBuilder($"{targetType.Name}{namePostfix}", moduleBuilder);

                case TypeCategories.Interface:
                    return DynamicTypeBuilderFactory.CreateInterfaceBuilder($"{targetType.Name}{namePostfix}", moduleBuilder, interfaces);

                case TypeCategories.Implementation:
                    return DynamicTypeBuilderFactory.CreateClassBuilder($"{targetType.Name}{namePostfix}", ctorParams, moduleBuilder, interfaces);

                default:
                    throw new Exception();
            }
        }

        private void BuildMatchType(IDynamicTypeBuilder typeBuilder, ServicePack retPack)
        {
            SetAttributes(typeBuilder, typeAttributes);

            if (typeCategory != TypeCategories.Implementation)
            {
                SetProperties(typeBuilder, retPack, targetType);

                SetMethods(typeBuilder, retPack);
            }
        }

        private void SetMethods(IDynamicTypeBuilder typeBuilder, ServicePack retPack)
        {
            var searchBindingFlags = GetSearchBindingFlags();

            var typeMethods = targetType.GetMethods(searchBindingFlags).Where(method => method.IsVirtual);

            foreach (var method in typeMethods)
            {
                var methodBuilder = typeBuilder.SetMethod(method.Name);

                SetMethodReturnType(retPack, method, methodBuilder);

                SetMethodParams(retPack, method, methodBuilder);

                SetAttributes(methodBuilder, allMembersAttributes);
            }
        }

        private BindingFlags GetSearchBindingFlags()
        {
            if (targetType.IsClass)
            {
                return BindingFlags.DeclaredOnly;
            }

            return BindingFlags.Public | BindingFlags.Instance;
        }

        private void SetMethodParams(ServicePack retPack, MethodInfo method, IDynamicMethodBuilder methodBuilder)
        {
            var methodParams = method.GetParameters();

            foreach (var param in methodParams)
            {
                var matchParamType = MapType(param.ParameterType, retPack);

                methodBuilder.SetParameter(matchParamType, param.Name);
            }
        }

        private void SetMethodReturnType(ServicePack retPack, MethodInfo method, IDynamicMethodBuilder methodBuilder)
        {
            if (!allMethodsVoid)
            {
                var matchReturnType = MapType(method.ReturnType, retPack);

                methodBuilder.SetReturnType(matchReturnType);
            }
        }

        private void SetProperties(IDynamicTypeBuilder typeBuilder, ServicePack retPack, Type type)
        {
            var typeProps = type.GetProperties();

            foreach (var prop in typeProps)
            {
                var matchType = MapType(prop.PropertyType, retPack);

                var propertyBuilder = typeBuilder.SetProperty(prop.Name, matchType);

                SetAttributes(propertyBuilder, allMembersAttributes);

                if (matchType != prop.PropertyType)
                {
                    MapSubTypes(prop.PropertyType, retPack);
                }
            }
        }

        private Type MapType(Type type, ServicePack retPack)
        {
            Type matchType = null;

            if (CheckMapPossiblity(type, out Type typeToMap))
            {
                if (retPack.RelatedTypes.Values.Any(sType => sType.Equals(typeToMap)))
                {
                    matchType = retPack.RelatedTypes.Single(kVT => kVT.Value.Equals(typeToMap)).Key;
                }
                else if (TryToGetFromGlobalContainer(type, out Type gottenMatchType))
                {
                    matchType = gottenMatchType;

                    retPack.AddRelatedType(matchType, type);
                }
                else
                {
                    var propMatcher = new ServiceMatcher(typeToMap, TypeCategories.Dto, optimizationPackage: optimizationPackage);

                    SetInvolvedTypesAttributes(propMatcher);

                    SetInvolvedTypeMembersAttributes(propMatcher);

                    var propServicePack = propMatcher.Pack();

                    matchType = propServicePack.MatchType;

                    AddRelatedTypesToRetPack(retPack, propServicePack);
                }
            }

            matchType = WrapForArraysOrGenerics(type, retPack, matchType);

            return matchType ?? type;
        }

        private bool TryToGetFromGlobalContainer(Type type, out Type gottenMatchedType)
        {
            var typeKeyName = $"{type.Name}{namePostfix}";

            gottenMatchedType = null;

            if (typeContainer.Contains(typeKeyName))
            {
                gottenMatchedType = typeContainer.Get(typeKeyName);
            }

            return (gottenMatchedType != null);
        }

        private Type WrapForArraysOrGenerics(Type type, ServicePack retPack, Type matchType)
        {
            Type retType = null;

            if (type.IsGenericType)
            {
                var typeToWrap = matchType ?? type.GetGenericArguments()[0];
                retType = type.GetGenericTypeDefinition().MakeGenericType(new[] { typeToWrap });
            }

            if (type.IsArray)
            {
                var typeToWrap = matchType ?? type.GetElementType();
                retType = typeToWrap.MakeArrayType();
            }

            if (matchType != null && retType != null)
            {
                retPack.AddRelatedType(retType, type);
            }

            return retType;
        }

        private void SetInvolvedTypeMembersAttributes(ServiceMatcher propMatcher)
        {
            foreach (var attribute in involvedTypeMembersAttributes)
            {
                propMatcher.SetAttributeForAllMembers(attribute.Item1, attribute.Item2, attribute.Item3);
                propMatcher.SetAttributeForAllInvolvedTypeMembers(attribute.Item1, attribute.Item2, attribute.Item3);
            }
        }

        private void SetInvolvedTypesAttributes(ServiceMatcher propMatcher)
        {
            foreach (var attribute in involvedAttributes)
            {
                propMatcher.SetAttributeOnType(attribute.Item1, attribute.Item2, attribute.Item3);
                propMatcher.SetAttributeForAllInvolvedTypes(attribute.Item1, attribute.Item2, attribute.Item3);
            }
        }

        private bool CheckMapPossiblity(Type type, out Type typeToMap)
        {
            typeToMap = PopGenericOrArray(type);

            typeToMap = typeToMap ?? type;

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


                if (CheckMapPossiblity(prop.PropertyType, out Type typeToMap))
                {
                    MapType(typeToMap, retPack);
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

        private void SetAttributes(
            IDynamicAttributeSetter attributeSetter,
            IList<Tuple<Type, IDictionary<Type, object>, IDictionary<string, object>>> attTuples)
        {
            foreach (var attribute in attTuples)
            {
                attributeSetter.SetAttribute(attribute.Item1, attribute.Item2, attribute.Item3);
            }
        }
    }
}
