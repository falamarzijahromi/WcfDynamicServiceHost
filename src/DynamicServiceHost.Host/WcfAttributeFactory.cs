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

                new AttributePack(
                    attributeType: typeof(ServiceBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(ServiceBehaviorAttribute.TransactionAutoCompleteOnSessionClose), true},
                        { nameof(ServiceBehaviorAttribute.TransactionIsolationLevel), isTransactional ? IsolationLevel.Serializable : IsolationLevel.ReadUncommitted},
                        { nameof(ServiceBehaviorAttribute.ReleaseServiceInstanceOnTransactionComplete), isTransactional},
                        { nameof(ServiceBehaviorAttribute.ConcurrencyMode), ConcurrencyMode.Single},
                        { nameof(ServiceBehaviorAttribute.InstanceContextMode), isTransactional ? InstanceContextMode.PerSession : InstanceContextMode.PerCall},
                        { nameof(ServiceBehaviorAttribute.EnsureOrderedDispatch), isTransactional},
                        { nameof(ServiceBehaviorAttribute.AutomaticSessionShutdown), true},
                    }),
            };
        }


        public static List<AttributePack> CreateForAllmembersConnectedAttributes(bool isTransactional = true)
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
                        { nameof(OperationBehaviorAttribute.TransactionScopeRequired), isTransactional},
                        { nameof(OperationBehaviorAttribute.ReleaseInstanceMode), isTransactional ? ReleaseInstanceMode.None : ReleaseInstanceMode.AfterCall},
                    }),

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