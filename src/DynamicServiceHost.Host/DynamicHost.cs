using DynamicServiceHost.Host.Abstracts;
using DynamicServiceHost.Host.WcfRequirements;
using DynamicWcfServiceHost.Shared.Abstracts;
using DynamicWcfServiceHost.Shared.DGenRequirements;
using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Messaging;
using System.ServiceModel;
using DynamicServiceHost.Matcher;
using ServicePack = DynamicWcfServiceHost.Shared.DGenRequirements.ServicePack;

namespace DynamicServiceHost.Host
{
    public class DynamicHost : IDisposable
    {
        private ServiceHost connectedHost;
        private ServiceHost disconnectedHost;

        private const string connectedPostFix = "_Connected";
        private const string disconnectedPostFix = "_Disconnected";

        private readonly Type serviceType;
        private readonly IHostContainer container;
        private readonly IOptimizationPackage optimizationPackage;

        public DynamicHost(
            Type serviceType, 
            IHostContainer container, 
            Type invokerType = null,
            IOptimizationPackage optimizationPackage = null)
        {
            this.serviceType = serviceType;
            this.container = container;
            this.optimizationPackage = optimizationPackage;

            RegisterInvokerToContainer(invokerType);
        }

        public void Dispose()
        {
            connectedHost.Close();
            disconnectedHost.Close();
        }

        public void Open()
        {
            connectedHost?.Open();
            disconnectedHost?.Open();
        }

        public void CreateConnectedHost(int? port = null, bool isTransactional = true)
        {
            var connectedServicePack = CreateEquivalentConnectedType(serviceType, isTransactional);

            connectedHost = CreateIocHosts(container, connectedServicePack.MatchType, port);
        }

        public void CreateDisconnectedHost()
        {
            //AssureOnQueueExists(serviceType);

            var disconnectedServicePack = CreateEquivalentDisconnectedType(serviceType);

            disconnectedHost = CreateIocHosts(container, disconnectedServicePack.MatchType, null);
        }


        private void AssureOnQueueExists(Type serviceType)
        {
            var queueAddress = $@"localhost\Private$\{serviceType.Name}";

            if (!MessageQueue.Exists(queueAddress))
            {
                var messageQueue = MessageQueue.Create(queueAddress, true);

                messageQueue.Authenticate = false;
                messageQueue.Label = queueAddress;

                messageQueue.Refresh();
                messageQueue.Close();
            }
        }

        private ServicePack CreateEquivalentDisconnectedType(Type contractType)
        {
            var onTypeAttributes = WcfAttributeFactory.CreateOnTypeAttributes();
            var forAllmembersAttributes = WcfAttributeFactory.CreateForAllmembersDisconnectedAttributes();
            var forAllInvolvedTypesAttributes = WcfAttributeFactory.CreateForAllInvolvedTypesAttributes();
            var forAllInvolvedTypeMembersAttributes = WcfAttributeFactory.CreateForAllInvolvedTypeMembersAttributes();

            return TypeFactory.CreateClassServicePack(
                type: contractType,
                typePostfix: disconnectedPostFix,
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes,
                optimizationPackage: optimizationPackage,
                allMethodsVoid: true);
        }

        private ServicePack CreateEquivalentConnectedType(Type contractType, bool isTransactional)
        {
            var onTypeAttributes = WcfAttributeFactory.CreateOnTypeAttributes(isTransactional);
            var forAllmembersAttributes = WcfAttributeFactory.CreateForAllmembersConnectedAttributes(isTransactional);
            var forAllInvolvedTypesAttributes = WcfAttributeFactory.CreateForAllInvolvedTypesAttributes();
            var forAllInvolvedTypeMembersAttributes = WcfAttributeFactory.CreateForAllInvolvedTypeMembersAttributes();

            return TypeFactory.CreateClassServicePack(
                type: contractType,
                typePostfix: connectedPostFix,
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                optimizationPackage: optimizationPackage,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes);
        }

        private ServiceHost CreateIocHosts(IHostContainer container, Type serviceType, int? port)
        {
            var instanceProvider = new HostInstanceProvider(container);

            var behavior = new HostInstanceProviderBehavior(instanceProvider);

            return new IocServiceHost(serviceType, behavior, port);
        }

        private void RegisterInvokerToContainer(Type invokerType)
        {
            invokerType = invokerType ?? typeof(DefaultInvoker);

            var invokerSetType = Evaluator<DefaultInvoker>.CreateDefaultEvaluatorType(invokerType);

            var invokationEvaluatorType = Evaluator<DefaultInvoker>.GetInvokationEvaluatorType();

            var typeMapper = new DefaultInvokerMapper();

            container.RegisterTransient(invokationEvaluatorType, invokerSetType);
            container.RegisterSingleton<IInvokerTypeMapper>(typeMapper);
        }
    }
}
