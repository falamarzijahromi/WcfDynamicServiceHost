using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.WcfRequirements;
using DynamicWcfServiceHost.Shared.Abstracts;
using DynamicWcfServiceHost.Shared.DGenRequirements;
using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Transactions;

namespace DynamicServiceHost.Host
{
    public class DynamicHost : IDisposable
    {
        private ServiceHost connectedHost;

        public DynamicHost(Type serviceType, IHostContainer container, Type invokerType = null, int? port = null)
        {
            invokerType = invokerType ?? typeof(DefaultInvoker);

            var connectedServicePack = CreateEquivalentConnectedType(serviceType);

            CreateIocHosts(container, connectedServicePack.MatchType, port);

            RegisterInvokerToContainer(container, invokerType, connectedServicePack);
        }

        public void Dispose()
        {
            connectedHost.Close();
        }

        public void Open()
        {
            connectedHost.Open();
        }

        private void CreateIocHosts(IHostContainer container, Type serviceType, int? port)
        {
            var instanceProvider = new HostInstanceProvider(container);

            var behavior = new HostInstanceProviderBehavior(instanceProvider);

            connectedHost = new IocServiceHost(serviceType, behavior, port);
        }

        private void RegisterInvokerToContainer(IHostContainer container, Type invokerType, ServicePack connectedServicePack)
        {
            var invokerSetType = Evaluator<DefaultInvoker>.CreateDefaultEvaluatorType(invokerType);

            var invokationEvaluatorType = Evaluator<DefaultInvoker>.GetInvokationEvaluatorType();

            var typeMapper = new DefaultInvokerMapper(connectedServicePack);

            container.RegisterTransient(invokationEvaluatorType, invokerSetType);
            container.RegisterSingleton<IInvokerTypeMapper>(typeMapper);
        }

        private ServicePack CreateEquivalentConnectedType(Type contractType)
        {
            var onTypeAttributes = CreateOnTypeConnectedAttributes();
            var forAllmembersAttributes = CreateForAllmembersConnectedAttributes();
            var forAllInvolvedTypesAttributes = CreateForAllInvolvedTypesConnectedAttributes();
            var forAllInvolvedTypeMembersAttributes = CreateForAllInvolvedTypeMembersConnectedAttributes();

            return TypeFactory.CreateClassServicePack(
                type: contractType,
                typePostfix: "_Connected",
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes);
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
                    attributeType: typeof(OperationContractAttribute)),

                new AttributePack(
                    attributeType: typeof(OperationBehaviorAttribute),
                    propsValuesMapping: new Dictionary<string, object>
                    {
                        { nameof(OperationBehaviorAttribute.TransactionAutoComplete), true },
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

        private static List<AttributePack> CreateOnTypeConnectedAttributes()
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
                        { nameof(ServiceBehaviorAttribute.TransactionAutoCompleteOnSessionClose), false},
                        { nameof(ServiceBehaviorAttribute.TransactionIsolationLevel), IsolationLevel.Serializable},
                        { nameof(ServiceBehaviorAttribute.ReleaseServiceInstanceOnTransactionComplete), true},
                        { nameof(ServiceBehaviorAttribute.ConcurrencyMode), ConcurrencyMode.Single},
                        { nameof(ServiceBehaviorAttribute.InstanceContextMode), InstanceContextMode.PerSession},
                        { nameof(ServiceBehaviorAttribute.EnsureOrderedDispatch), true},
                    }),
            };
        }
    }
}
