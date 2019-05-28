using System;
using System.Collections.Generic;

namespace DynamicWcfServiceHost.Shared.Factories
{
    public class AttributePack
    {
        public AttributePack(
            Type attributeType, 
            IDictionary<Type, object> ctorParamsMapping, 
            IDictionary<string, object> propsValuesMapping)
        {
            AttributeType = attributeType;
            CtorParamsMapping = ctorParamsMapping;
            PropsValuesMapping = propsValuesMapping;
        }

        public Type AttributeType { get; }
        public IDictionary<Type, object> CtorParamsMapping { get; }
        public IDictionary<string, object> PropsValuesMapping { get; }
    }
}
