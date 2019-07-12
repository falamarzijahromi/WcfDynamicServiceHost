using DynamicWcfServiceHost.Shared.DGenRequirements;
using DynamicWcfServiceHost.Shared.Factories;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using DynamicServiceHost.Matcher;
using ServicePack = DynamicWcfServiceHost.Shared.DGenRequirements.ServicePack;

namespace DynamicWcfServiceHost.Proxy
{
    public class ChannelFactory
    {
        private const string connectedPostFix = "_Connected";
        private const string disconnectedPostFix = "_Disconnected";

        private readonly Type serviceType;
        private readonly Type invoker;
        private readonly IOptimizationPackage optimizationPackage;

        public ChannelFactory(
            Type serviceType, 
            IOptimizationPackage optimizationPackage = null, 
            Type invoker = null)
        {
            this.serviceType = serviceType;
            this.optimizationPackage = optimizationPackage;
            this.invoker = invoker ?? typeof(DefaultInvoker);
        }

        public object CreateConnectedChannel(
            ITypeCacher typeCacher,
            int? arbitaryPort = null, 
            bool isTransactional = true)
        {
            var connectedServicePack = CreateEquivalentConnectedType(serviceType, typeCacher, isTransactional);

            var connectedServiceTypeWrapper = CreateEquivalentWrapperType(serviceType,
                new Dictionary<string, Type>
                {
                    {"connectedServiceType", connectedServicePack.MatchType},
                }, typeCacher, connectedPostFix);

            var connectedServiceObject = CreateDefaultChannel(arbitaryPort, connectedServicePack.MatchType);

            var defaultEvluatorObject = CreateDefaultInvoker(invoker);

            var returnObject = CreateReturnObject(connectedServiceTypeWrapper, defaultEvluatorObject, connectedServiceObject);

            return returnObject;
        }

        public object CreateDisconnectedChannel(ITypeCacher typeCacher)
        {
            var disconnectedServicePack = CreateEquivalentDisconnectedType(serviceType, typeCacher);

            var disconnectedServiceTypeWrapper = CreateEquivalentWrapperType(serviceType,
                new Dictionary<string, Type>
                {
                    {"disconnectedServiceType", disconnectedServicePack.MatchType},
                }, typeCacher, disconnectedPostFix);

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

        private ServicePack CreateEquivalentConnectedType(
            Type contractType,
            ITypeCacher typeCacher,
            bool isTransactional)
        {
            var keyObject = $"{serviceType.FullName}-Connected:{isTransactional}";

            if (typeCacher.ContainesKey(keyObject))
            {
                return (ServicePack)typeCacher.Get(keyObject);
            }

            var onTypeAttributes = WcfAttributeFactory.CreateOnTypeAttributes(isTransactional);
            var forAllmembersAttributes = WcfAttributeFactory.CreateForAllmembersConnectedAttributes(isTransactional);
            var forAllInvolvedTypesAttributes = WcfAttributeFactory.CreateForAllInvolvedTypesAttributes();
            var forAllInvolvedTypeMembersAttributes = WcfAttributeFactory.CreateForAllInvolvedTypeMembersAttributes();

            var servicePack = TypeFactory.CreateInterfaceServicePack(
                type: contractType,
                typePostfix: connectedPostFix,
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                optimizationPackage: optimizationPackage,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes);

            typeCacher.Hold(keyObject, servicePack);

            return servicePack;
        }

        private ServicePack CreateEquivalentDisconnectedType(Type contractType, ITypeCacher typeCacher)
        {
            var keyObject = $"{serviceType.FullName}-Disconnected";

            if (typeCacher.ContainesKey(keyObject))
            {
                return (ServicePack)typeCacher.Get(keyObject);
            }

            var onTypeAttributes = WcfAttributeFactory.CreateOnTypeAttributes();
            var forAllmembersAttributes = WcfAttributeFactory.CreateForAllmembersDisconnectedAttributes();
            var forAllInvolvedTypesAttributes = WcfAttributeFactory.CreateForAllInvolvedTypesAttributes();
            var forAllInvolvedTypeMembersAttributes = WcfAttributeFactory.CreateForAllInvolvedTypeMembersAttributes();

            var servicePack = TypeFactory.CreateInterfaceServicePack(
                type: contractType,
                typePostfix: disconnectedPostFix,
                onType: onTypeAttributes,
                forAllmembers: forAllmembersAttributes,
                forAllInvolvedTypes: forAllInvolvedTypesAttributes,
                optimizationPackage: optimizationPackage,
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes,
                allMethodsVoid: true);

            typeCacher.Hold(keyObject, servicePack);

            return servicePack;
        }

        private Type CreateEquivalentWrapperType(
            Type contractType,
            IDictionary<string, Type> extraCtorParams,
            ITypeCacher typeCacher,
            string wrapperMode)
        {
            var keyObject = $"{contractType.FullName}-{wrapperMode}-Wrapper";

            if (typeCacher.ContainesKey(keyObject))
            {
                return (Type)typeCacher.Get(keyObject);
            }

            var wrapperType = TypeFactory.CreateImplementationServicePack(
                    type: contractType,
                    typePostfix: $"{wrapperMode}_Wrapper",
                    interfaces: new[] { contractType, typeof(IDisposable) },
                    optimizationPackage: optimizationPackage,
                    extraCtorParams: extraCtorParams)
                .MatchType;

            typeCacher.Hold(keyObject, wrapperType);

            return wrapperType;
        }
    }
}
