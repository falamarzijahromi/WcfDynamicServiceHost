using System;
using System.Collections.Generic;

namespace DynamicWcfServiceHost.Shared.Factories
{
    public class AttributePack
    {
        public AttributePack(
            Type attributeType, 
            IDictionary<Type, object> ctorParamsMapping = null, 
            IDictionary<string, object> propsValuesMapping = null)
        {
            AttributeType = attributeType;
            CtorParamsMapping = ctorParamsMapping ?? new Dictionary<Type, object>();
            PropsValuesMapping = propsValuesMapping ?? new Dictionary<string, object>();
        }

        public Type AttributeType { get; }
        public IDictionary<Type, object> CtorParamsMapping { get; }
        public IDictionary<string, object> PropsValuesMapping { get; }
    }
}
