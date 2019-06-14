using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DynamicWcfServiceHost.Proxy
{
    public static class WcfAttributeFactory
    {
        public static List<AttributePack> CreateForAllInvolvedTypeMembersAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(DataMemberAttribute))
            };
        }
        public static List<AttributePack> CreateForAllInvolvedTypesAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(DataContractAttribute))
            };
        }
        public static List<AttributePack> CreateOnTypeAttributes(bool isTransactional = true)
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(ServiceContractAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(ServiceContractAttribute.SessionMode), isTransactional ? SessionMode.Required : SessionMode.NotAllowed},
                    }),
            };
        }


        public static List<AttributePack> CreateForAllmembersConnectedAttributes(bool isTransactional = false)
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(OperationContractAttribute)),

                new AttributePack(
                    attributeType: typeof(TransactionFlowAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>
                    {
                        {typeof(TransactionFlowOption), isTransactional ? TransactionFlowOption.Mandatory : TransactionFlowOption.NotAllowed}
                    }),
            };
        }
        public static List<AttributePack> CreateForAllmembersDisconnectedAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(OperationContractAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        {nameof(OperationContractAttribute.IsOneWay), true },
                    }),
            };
        }
    }
}