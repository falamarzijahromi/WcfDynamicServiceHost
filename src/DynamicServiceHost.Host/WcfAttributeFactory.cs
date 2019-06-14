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
            var serviceContract = new AttributePack(
                attributeType: typeof(ServiceContractAttribute),
                propsValuesMapping: new Dictionary<string, object>
                {
                    {
                        nameof(ServiceContractAttribute.SessionMode),
                        isTransactional ? SessionMode.Required : SessionMode.Allowed
                    },
                });

            AttributePack serviceBehavior = null;

            if (isTransactional)
            {
                serviceBehavior = new AttributePack(
                        attributeType: typeof(ServiceBehaviorAttribute),
                        propsValuesMapping: new Dictionary<string, object>
                        {
                            {nameof(ServiceBehaviorAttribute.TransactionAutoCompleteOnSessionClose), true},
                            {
                                nameof(ServiceBehaviorAttribute.TransactionIsolationLevel), IsolationLevel.Serializable
                            },
                            {nameof(ServiceBehaviorAttribute.ReleaseServiceInstanceOnTransactionComplete), true},
                            {nameof(ServiceBehaviorAttribute.ConcurrencyMode), ConcurrencyMode.Single},
                            {
                                nameof(ServiceBehaviorAttribute.InstanceContextMode), InstanceContextMode.PerSession
                            },
                            {nameof(ServiceBehaviorAttribute.EnsureOrderedDispatch), true},
                            {nameof(ServiceBehaviorAttribute.AutomaticSessionShutdown), true},
                        });
            }
            else
            {
                serviceBehavior = new AttributePack(
                    attributeType: typeof(ServiceBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        {nameof(ServiceBehaviorAttribute.ConcurrencyMode), ConcurrencyMode.Single},
                        {
                            nameof(ServiceBehaviorAttribute.InstanceContextMode),
                            isTransactional ? InstanceContextMode.PerSession : InstanceContextMode.PerCall
                        },
                        {nameof(ServiceBehaviorAttribute.AutomaticSessionShutdown), true},
                    });
            }

            return new List<AttributePack> { serviceContract, serviceBehavior };
        }


        public static List<AttributePack> CreateForAllmembersConnectedAttributes(bool isTransactional = true)
        {
            var operationContract = new AttributePack(
                attributeType: typeof(OperationContractAttribute));

            AttributePack operationBehavior = null;

            if (isTransactional)
            {
                operationBehavior = new AttributePack(
                    attributeType: typeof(OperationBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        {nameof(OperationBehaviorAttribute.TransactionAutoComplete), false},
                        {nameof(OperationBehaviorAttribute.TransactionScopeRequired), true},
                        {
                            nameof(OperationBehaviorAttribute.ReleaseInstanceMode), ReleaseInstanceMode.None
                        },
                    });
            }
            else
            {
                operationBehavior = new AttributePack(
                    attributeType: typeof(OperationBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        {nameof(OperationBehaviorAttribute.TransactionScopeRequired), false},
                        {
                            nameof(OperationBehaviorAttribute.ReleaseInstanceMode), ReleaseInstanceMode.AfterCall
                        },
                    });
            }

            AttributePack transactionFlow = null;

            if (isTransactional)
            {
                transactionFlow = new AttributePack(
                    attributeType: typeof(TransactionFlowAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>
                    {
                        {
                            typeof(TransactionFlowOption), TransactionFlowOption.Mandatory
                        }
                    });
            }
            else
            {
                transactionFlow = new AttributePack(
                    attributeType: typeof(TransactionFlowAttribute),
                    ctorParamsMapping: new Dictionary<Type, object>
                    {
                        {
                            typeof(TransactionFlowOption), TransactionFlowOption.NotAllowed
                        }
                    });
            }

            return new List<AttributePack> { operationContract, operationBehavior, transactionFlow };
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