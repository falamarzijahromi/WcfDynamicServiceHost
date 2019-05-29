using DynamicTypeGenerator.Invokations;
using System;

namespace DynamicWcfServiceHost.Shared.DGenRequirements
{
    public class ArgInfo
    {
        public string ArgName { get; private set; }
        public Type ArgType { get; private set; }
        public object ArgObject { get; private set; }

        public static implicit operator ArgInfo(DynamicTypeGenerator.Invokations.ArgInfo arg)
        {
            return new ArgInfo
            {
                ArgName = arg.ParamName,
                ArgType = arg.ParamType,
                ArgObject = arg.ParamObject,
            };
        }
    }
}
