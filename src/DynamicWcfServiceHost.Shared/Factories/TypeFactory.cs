using DynamicServiceHost.Matcher;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ServicePack = DynamicWcfServiceHost.Shared.DGenRequirements.ServicePack;

namespace DynamicWcfServiceHost.Shared.Factories
{
    public class TypeFactory
    {
        public static ServicePack CreateInterfaceServicePack(
            Type type,
            string typePostfix,
            IList<AttributePack> onType,
            IList<AttributePack> forAllmembers,
            IList<AttributePack> forAllInvolvedTypes,
            IList<AttributePack> forAllInvolvedTypeMembers,
            IOptimizationPackage optimizationPackage = null,
            bool allMethodsVoid = false)
        {
            return CreateServicePack(type, TypeCategories.Interface, typePostfix, onType,
                forAllmembers, forAllInvolvedTypes, forAllInvolvedTypeMembers, allMethodsVoid: allMethodsVoid, optimizationPackage: optimizationPackage);
        }

        public static ServicePack CreateClassServicePack(
            Type type,
            string typePostfix,
            IList<AttributePack> onType,
            IList<AttributePack> forAllmembers,
            IList<AttributePack> forAllInvolvedTypes,
            IList<AttributePack> forAllInvolvedTypeMembers,
            IDictionary<string, Type> extraCtorParams = null,
            IOptimizationPackage optimizationPackage = null,
            bool allMethodsVoid = false)
        {
            return CreateServicePack(
                type,
                TypeCategories.Class,
                typePostfix,
                onType,
                forAllmembers,
                forAllInvolvedTypes,
                forAllInvolvedTypeMembers,
                extraCtorParams,
                optimizationPackage,
                allMethodsVoid: allMethodsVoid);
        }

        public static ServicePack CreateImplementationServicePack(
            Type type, string typePostfix,
            Type[] interfaces,
            IOptimizationPackage optimizationPackage = null,
            IDictionary<string, Type> extraCtorParams = null)
        {
            return CreateServicePack(
                type, TypeCategories.Implementation, typePostfix,
                new List<AttributePack>(),
                new List<AttributePack>(),
                new List<AttributePack>(),
                new List<AttributePack>(), extraCtorParams, interfaces: interfaces, optimizationPackage: optimizationPackage);
        }

        private static ServicePack CreateServicePack(
            Type type,
            TypeCategories typeCategory,
            string typePostfix,
            IList<AttributePack> onType,
            IList<AttributePack> forAllmembers,
            IList<AttributePack> forAllInvolvedTypes,
            IList<AttributePack> forAllInvolvedTypeMembers,
            IDictionary<string, Type> extraCtorParams = null,
            IOptimizationPackage optimizationPackage = null,
            bool allMethodsVoid = false,

            params Type[] interfaces)
        {
            var matcher = new ServiceMatcher(type, typeCategory, typePostfix, extraCtorParams, interfaces: interfaces, allMethodsVoid: allMethodsVoid, optimizationPackage: optimizationPackage);

            SetOnTypeAttributes(matcher, onType);
            SetForAllmembersAttributes(matcher, forAllmembers);
            SetForAllInvolvedTypesAttributes(matcher, forAllInvolvedTypes);
            SetForAllInvolvedTypeMembersAttributes(matcher, forAllInvolvedTypeMembers);

            return matcher.Pack();
        }

        private static void SetForAllInvolvedTypeMembersAttributes(ServiceMatcher matcher, IList<AttributePack> forAllInvolvedTypeMembers)
        {
            foreach (var attribute in forAllInvolvedTypeMembers)
            {
                matcher.SetAttributeForAllInvolvedTypeMembers(attribute.AttributeType, attribute.CtorParamsMapping,
                    attribute.PropsValuesMapping);
            }
        }

        private static void SetForAllInvolvedTypesAttributes(ServiceMatcher matcher, IList<AttributePack> forAllInvolvedTypes)
        {
            foreach (var attribute in forAllInvolvedTypes)
            {
                matcher.SetAttributeForAllInvolvedTypes(attribute.AttributeType, attribute.CtorParamsMapping,
                    attribute.PropsValuesMapping);
            }
        }

        private static void SetForAllmembersAttributes(ServiceMatcher matcher, IList<AttributePack> forAllmembers)
        {
            foreach (var attribute in forAllmembers)
            {
                matcher.SetAttributeForAllMembers(attribute.AttributeType, attribute.CtorParamsMapping,
                    attribute.PropsValuesMapping);
            }
        }

        private static void SetOnTypeAttributes(ServiceMatcher matcher, IList<AttributePack> onType)
        {
            foreach (var attribute in onType)
            {
                matcher.SetAttributeOnType(attribute.AttributeType, attribute.CtorParamsMapping,
                    attribute.PropsValuesMapping);
            }
        }
    }
}