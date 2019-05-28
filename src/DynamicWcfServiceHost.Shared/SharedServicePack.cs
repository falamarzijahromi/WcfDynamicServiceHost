using System;
using System.Collections.Generic;
using DynamicServiceHost.Matcher;

namespace DynamicWcfServiceHost.Shared
{
    public class SharedServicePack
    {
        public Type MatchType { get; private set; }
        public Dictionary<Type, Type> RelatedTypes { get; private set; }

        public static implicit operator SharedServicePack(ServicePack pack)
        {
            return new SharedServicePack
            {
                MatchType = pack.MatchType,
                RelatedTypes = pack.RelatedTypes,
            };
        }
    }
}
