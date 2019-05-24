using System;
using System.Collections.Generic;

namespace DynamicServiceHost.Matcher
{
    public class ServicePack
    {
        private readonly IDictionary<Type, Type> relatedTypes;

        public ServicePack(Type matchType, IDictionary<Type, Type> relatedTypes)
        {
            MatchType = matchType;

            this.relatedTypes = relatedTypes;
        }

        public Type MatchType { get; }

        public Dictionary<Type, Type> RelatedTypes => new Dictionary<Type, Type>(relatedTypes);
    }
}
