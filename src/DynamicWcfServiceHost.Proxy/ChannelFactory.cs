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
        public static object CreateConnectedChannel(Type serviceType, int? arbitaryPort = null, Type invoker = null)
        {
            invoker = invoker ?? typeof(DefaultInvoker);

            var connectedServicePack = CreateEquivalentConnectedType(serviceType);

            var connectedServiceTypeWrapper = CreateEquivalentWrapperConnectedType(serviceType,
                new Dictionary<string, Type>
                {
                    {"connectedServiceType", connectedServicePack.MatchType},
                });

            var connectedServiceObject = CreateDefaultChannel(arbitaryPort, connectedServicePack.MatchType);

            var defaultEvluatorObject = CreateDefaultInvoker(invoker, connectedServicePack);

            var returnObject = CreateReturnObject(connectedServiceTypeWrapper, defaultEvluatorObject, connectedServiceObject);

            return returnObject;
        }

        private static object CreateDefaultInvoker(Type invoker, ServicePack servicePack)
        {
            var invokerTypeMapper = new DefaultInvokerMapper(servicePack);

            return Evaluator<DefaultInvoker>.CreateDefaultEvaluator(invoker, invokerTypeMapper);
        }

        private static object CreateReturnObject(Type connectedServiceTypeWrapper, object defaultEvluatorObject, object connectedServiceObject)
        {
            var wrapper = Activator.CreateInstance(connectedServiceTypeWrapper,
                new[] {defaultEvluatorObject, connectedServiceObject, null});

            return wrapper;
        }

        private static object CreateDefaultChannel(int? arbitaryPort, Type connectedServiceType)
        {
            var notSetType = typeof(ChannelFactory<>);

            var setType = notSetType.MakeGenericType(connectedServiceType);

            var createChannelMethod = setType.GetMethod("CreateChannel", new Type[] { typeof(Type), typeof(int?) });

            var connectedServiceObject = createChannelMethod.Invoke(null, new object[] { connectedServiceType, arbitaryPort });

            return connectedServiceObject;
        }

        private static ServicePack CreateEquivalentConnectedType(Type contractType)
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
                forAllInvolvedTypeMembers: forAllInvolvedTypeMembersAttributes);
        }

        private static Type CreateEquivalentWrapperConnectedType(Type contractType, IDictionary<string, Type> extraCtorParams)
        {
            return TypeFactory.CreateImplementationServicePack(
                    type: contractType,
                    typePostfix: "_Wrapper",
                    extraCtorParams: extraCtorParams,
                    @interface: contractType)
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
