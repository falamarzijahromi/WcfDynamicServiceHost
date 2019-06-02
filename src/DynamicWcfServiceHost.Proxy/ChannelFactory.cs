using DynamicWcfServiceHost.Shared.DGenRequirements;
using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DynamicWcfServiceHost.Proxy
{
    public class ChannelFactory
    {
        private const string connectedPostFix = "_Connected";
        private const string disconnectedPostFix = "_Disconnected";

        private readonly Type serviceType;
        private readonly Type invoker;

        public ChannelFactory(Type serviceType, Type invoker = null)
        {
            this.serviceType = serviceType;
            this.invoker = invoker ?? typeof(DefaultInvoker);
        }

        public object CreateConnectedChannel(int? arbitaryPort = null)
        {
            var connectedServicePack = CreateEquivalentConnectedType(serviceType);

            var connectedServiceTypeWrapper = CreateEquivalentWrapperType(serviceType,
                new Dictionary<string, Type>
                {
                    {"connectedServiceType", connectedServicePack.MatchType},
                });

            var connectedServiceObject = CreateDefaultChannel(arbitaryPort, connectedServicePack.MatchType);

            var defaultEvluatorObject = CreateDefaultInvoker(invoker);

            var returnObject = CreateReturnObject(connectedServiceTypeWrapper, defaultEvluatorObject, connectedServiceObject);

            return returnObject;
        }

        public object CreateDisconnectedChannel()
        {
            var disconnectedServicePack = CreateEquivalentDisconnectedType(serviceType);

            var disconnectedServiceTypeWrapper = CreateEquivalentWrapperType(serviceType,
                new Dictionary<string, Type>
                {
                    {"disconnectedServiceType", disconnectedServicePack.MatchType},
                });

            var connectedServiceObject = CreateDefaultChannel(null, disconnectedServicePack.MatchType);

            var defaultEvluatorObject = CreateDefaultInvoker(invoker);

            var returnObject = CreateReturnObject(disconnectedServiceTypeWrapper, defaultEvluatorObject, connectedServiceObject);

            return returnObject;
        }

        private object CreateDefaultInvoker(Type invoker)
        {
            var invokerTypeMapper = new DefaultInvokerMapper();

            return Evaluator<DefaultInvoker>.CreateDefaultEvaluator(invoker, invokerTypeMapper);
        }

        private object CreateReturnObject(Type serviceTypeWrapper, object defaultEvluatorObject, object serviceObject)
        {
            var wrapper = Activator.CreateInstance(serviceTypeWrapper,
                new[] { defaultEvluatorObject, serviceObject, null });

            return wrapper;
        }

        private object CreateDefaultChannel(int? arbitaryPort, Type serviceType)
        {
            var notSetType = typeof(ChannelFactory<>);

            var setType = notSetType.MakeGenericType(serviceType);

            var createChannelMethod = setType.GetMethod("CreateChannel", new Type[] { typeof(Type), typeof(int?) });

            var connectedServiceObject = createChannelMethod.Invoke(null, new object[] { serviceType, arbitaryPort });

            return connectedServiceObject;
        }

        private ServicePack CreateEquivalentConnectedType(Type contractType)
        {
            var onTypeAttributes = WcfAttributeFactory.CreateOnTypeAttributes();
            var forAllmembersAttributes = WcfAttributeFactory.CreateForAllmembersConnectedAttributes();
            var forAllInvolvedTypesAttributes = WcfAttributeFactory.CreateForAllInvolvedTypesAttributes();
            var forAllInvolvedTypeMembersAttributes = WcfAttributeFactory.CreateForAllInvolvedTypeMembersAttributes();

            return TypeFactory.CreateInterfaceServicePack(
                type: contractType,
                typePostfix: connectedPostFix,
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes);
        }

        private ServicePack CreateEquivalentDisconnectedType(Type contractType)
        {
            var onTypeAttributes = WcfAttributeFactory.CreateOnTypeAttributes();
            var forAllmembersAttributes = WcfAttributeFactory.CreateForAllmembersDisconnectedAttributes();
            var forAllInvolvedTypesAttributes = WcfAttributeFactory.CreateForAllInvolvedTypesAttributes();
            var forAllInvolvedTypeMembersAttributes = WcfAttributeFactory.CreateForAllInvolvedTypeMembersAttributes();

            return TypeFactory.CreateInterfaceServicePack(
                type: contractType,
                typePostfix: disconnectedPostFix,
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes,
                allMethodsVoid: true);
        }

        private Type CreateEquivalentWrapperType(Type contractType, IDictionary<string, Type> extraCtorParams)
        {
            return TypeFactory.CreateImplementationServicePack(
                    type: contractType,
                    typePostfix: "_Wrapper",
                    interfaces: new[] { contractType, typeof(IDisposable) },
                    extraCtorParams: extraCtorParams)
                .MatchType;
        }
    }
}
