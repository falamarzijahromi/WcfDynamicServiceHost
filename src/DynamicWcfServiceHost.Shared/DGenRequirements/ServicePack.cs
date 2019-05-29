using System;
using System.Collections.Generic;

namespace DynamicWcfServiceHost.Shared.DGenRequirements
{
    public class ServicePack
    {
        public Type MatchType { get; private set; }
        public Dictionary<Type, Type> RelatedTypes { get; private set; }

        public static implicit operator ServicePack(DynamicServiceHost.Matcher.ServicePack pack)
        {
            return new ServicePack
            {
                MatchType = pack.MatchType,
                RelatedTypes = pack.RelatedTypes,
            };
        }
    }
}
