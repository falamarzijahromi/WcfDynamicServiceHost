using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Transactions;

namespace DynamicServiceHost.Host
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
        public static List<AttributePack> CreateOnTypeAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(ServiceContractAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(ServiceContractAttribute.SessionMode), SessionMode.Required},
                    }),

                new AttributePack(
                    attributeType: typeof(ServiceBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(ServiceBehaviorAttribute.TransactionAutoCompleteOnSessionClose), true},
                        { nameof(ServiceBehaviorAttribute.TransactionIsolationLevel), IsolationLevel.Serializable},
                        { nameof(ServiceBehaviorAttribute.ReleaseServiceInstanceOnTransactionComplete), true},
                        { nameof(ServiceBehaviorAttribute.ConcurrencyMode), ConcurrencyMode.Single},
                        { nameof(ServiceBehaviorAttribute.InstanceContextMode), InstanceContextMode.PerSession},
                        { nameof(ServiceBehaviorAttribute.EnsureOrderedDispatch), true},
                        { nameof(ServiceBehaviorAttribute.AutomaticSessionShutdown), true},
                    }),
            };
        }


        public static List<AttributePack> CreateForAllmembersConnectedAttributes()
        {
            return new List<AttributePack>
            {
                new AttributePack(
                    attributeType: typeof(OperationContractAttribute)),

                new AttributePack(
                    attributeType: typeof(OperationBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(OperationBehaviorAttribute.TransactionAutoComplete), false },
                        { nameof(OperationBehaviorAttribute.TransactionScopeRequired), true},
                        { nameof(OperationBehaviorAttribute.ReleaseInstanceMode), ReleaseInstanceMode.None},
                    }),

                new AttributePack(
                    attributeType: typeof(TransactionFlowAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>
                    {
                        {typeof(TransactionFlowOption), TransactionFlowOption.Mandatory }
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

                new AttributePack(
                    attributeType: typeof(OperationBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(OperationBehaviorAttribute.TransactionAutoComplete), false },
                        { nameof(OperationBehaviorAttribute.TransactionScopeRequired), true},
                        { nameof(OperationBehaviorAttribute.ReleaseInstanceMode), ReleaseInstanceMode.None},
                    }),
            };
        }
    }
}