using System;
using System.Collections.Generic;

namespace DynamicServiceHost.Matcher
{
    public class ServicePack
    {
        private readonly IDictionary<Type, Type> relatedTypes;

        internal ServicePack()
        {
            relatedTypes = new Dictionary<Type, Type>();
        }

        internal void AddRelatedType(Type keyType, Type valueType)
        {
            if (!relatedTypes.Values.Contains(valueType))
            {
                relatedTypes.Add(keyType, valueType);
            }
        }

        internal void SetMatchType(Type matchType)
        {
            MatchType = matchType;
        }

        public Type MatchType { get; private set; }

        public Dictionary<Type, Type> RelatedTypes => new Dictionary<Type, Type>(relatedTypes);
    }
}
