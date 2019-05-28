using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.DGenRequirements;
using DynamicServiceHost.Host.WcfRequirements;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using DynamicTypeGenerator.Abstracts;
using DynamicWcfServiceHost.Shared.Factories;

namespace DynamicServiceHost.Host
{
    public class DynamicHost : IDisposable
    {
        private ServiceHost connectedHost;

        public DynamicHost(Type serviceType, IHostContainer container, Type invokerType = null)
        {
            invokerType = invokerType ?? typeof(DefaultHostInvoker);

            var connectedServiceType = CreateEquivalentConnectedType(serviceType);

            CreateIocHosts(container, connectedServiceType);

            RegisterInvokerToContainer(container, invokerType);
        }

        public void Dispose()
        {
            connectedHost.Close();
        }

        public void Open()
        {
            connectedHost.Open();
        }

        private void CreateIocHosts(IHostContainer container, Type serviceType)
        {
            var instanceProvider = new HostInstanceProvider(container);

            var behavior = new HostInstanceProviderBehavior(instanceProvider);

            connectedHost = new IocServiceHost(serviceType, behavior);
        }

        private void RegisterInvokerToContainer(IHostContainer container, Type invokerType)
        {
            var nonSetEvaluatorType = typeof(Evaluator<>);

            var setEvaluatorType = nonSetEvaluatorType.MakeGenericType(new[] { invokerType });

            container.RegisterTransient(typeof(IInvokationEvaluator), setEvaluatorType);
        }

        private Type CreateEquivalentConnectedType(Type contractType)
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
