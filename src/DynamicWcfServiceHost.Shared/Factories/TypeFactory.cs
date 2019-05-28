using DynamicServiceHost.Matcher;
using System;
using System.Collections.Generic;

namespace DynamicWcfServiceHost.Shared.Factories
{
    public class TypeFactory
    {
        public static SharedServicePack CreateInterfaceServicePack(
            Type type,
            string typePostfix,
            IList<AttributePack> onType,
            IList<AttributePack> forAllmembers,
            IList<AttributePack> forAllInvolvedTypes,
            IList<AttributePack> forAllInvolvedTypeMembers)
        {
            return CreateServicePack(type, TypeCategories.Interface, typePostfix, onType,
                forAllmembers, forAllInvolvedTypes, forAllInvolvedTypeMembers);
        }

        public static SharedServicePack CreateClassServicePack(
            Type type,
            string typePostfix,
            IList<AttributePack> onType,
            IList<AttributePack> forAllmembers,
            IList<AttributePack> forAllInvolvedTypes,
            IList<AttributePack> forAllInvolvedTypeMembers)
        {
            return CreateServicePack(type, TypeCategories.Class, typePostfix, onType,
                forAllmembers, forAllInvolvedTypes, forAllInvolvedTypeMembers);
        }

        private static SharedServicePack CreateServicePack(
            Type type,
            TypeCategories typeCategory,
            string typePostfix,
            IList<AttributePack> onType,
            IList<AttributePack> forAllmembers,
            IList<AttributePack> forAllInvolvedTypes,
            IList<AttributePack> forAllInvolvedTypeMembers)
        {
            var matcher = new ServiceMatcher(type, typeCategory, typePostfix);

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