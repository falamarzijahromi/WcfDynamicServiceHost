using System;
using System.Collections.Generic;

namespace DynamicWcfServiceHost.Shared.DGenRequirements
{
    public class InvokationContext
    {
        public Type ReturnType { get; internal set; }
        public List<ArgInfo> Parameters { get; internal set; }
        public List<ArgInfo> Fields { get; internal set; }
        public string MethodName { get; internal set; }
    }
}
