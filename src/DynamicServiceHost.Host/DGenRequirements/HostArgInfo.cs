using System;
using DynamicTypeGenerator.Invokations;

namespace DynamicServiceHost.Host.DGenRequirements
{
    public class HostArgInfo
    {
        public string ArgName { get; private set; }
        public Type ArgType { get; private set; }
        public object ArgObject { get; private set; }

        public static implicit operator HostArgInfo(ArgInfo arg)
        {
            return new HostArgInfo
            {
                ArgName = arg.ParamName,
                ArgType = arg.ParamType,
                ArgObject = arg.ParamObject,
            };
        }
    }
}
