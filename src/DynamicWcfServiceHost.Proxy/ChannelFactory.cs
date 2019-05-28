using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DynamicWcfServiceHost.Proxy
{
    public class ChannelFactory
    {
        public static object CreateConnectedChannel(Type serviceType)
        {
            var connectedServiceType = CreateEquivalentConnectedType(serviceType);

            var notSetType = typeof(ChannelFactory<>);

            var setType = notSetType.MakeGenericType(connectedServiceType);

            var createChannelMethod = setType.GetMethod("CreateChannel", new Type[] { typeof(Type) });

            return createChannelMethod.Invoke(null, new[] { connectedServiceType });
        }

        private static Type CreateEquivalentConnectedType(Type contractType)
        {
            var onTypeAttributes = CreateOnTypeConnectedAttributes();
            var forAllmembersAttributes = CreateForAllmembersConnectedAttributes();
            var forAllInvolvedTypesAttributes = CreateForAllInvolvedTypesConnectedAttributes();
            var forAllInvolvedTypeMembersAttributes = CreateForAllInvolvedTypeMembersConnectedAttributes();

            return TypeFactory.CreateInterfaceServicePack(
                type: contractType,
                typePostfix: "_Connected",
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes)
                .MatchType;
        }

        private static List<AttributePack> CreateForAllInvolvedTypeMembersConnectedAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(DataMemberAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>(),
                    propsValuesMapping: new Dictionary<string, object>()),
            };
        }

        private static List<AttributePack> CreateForAllInvolvedTypesConnectedAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(DataContractAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>(),
                    propsValuesMapping: new Dictionary<string, object>()),
            };
        }

        private static List<AttributePack> CreateForAllmembersConnectedAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(OperationContractAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>(),
                    propsValuesMapping: new Dictionary<string, object>()),
            };
        }

        private static List<AttributePack> CreateOnTypeConnectedAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(ServiceContractAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>(),
                    propsValuesMapping: new Dictionary<string, object>()),
            };
        }
    }
}
