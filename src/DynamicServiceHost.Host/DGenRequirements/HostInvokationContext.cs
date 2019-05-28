using System;
using System.Collections.Generic;

namespace DynamicServiceHost.Host.DGenRequirements
{
    public class HostInvokationContext
    {
        public Type ReturnType { get; internal set; }
        public List<HostArgInfo> Parameters { get; internal set; }
        public List<HostArgInfo> Fields { get; internal set; }
        public string MethodName { get; internal set; }
    }
}
