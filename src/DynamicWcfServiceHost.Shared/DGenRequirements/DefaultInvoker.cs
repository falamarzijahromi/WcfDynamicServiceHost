using System.Collections.Generic;
using DynamicWcfServiceHost.Shared.Abstracts;
using System.Linq;

namespace DynamicWcfServiceHost.Shared.DGenRequirements
{
    public class DefaultInvoker : IInvoker
    {
        private readonly IInvokerTypeMapper invokerTypeMapper;

        public DefaultInvoker(IInvokerTypeMapper invokerTypeMapper)
        {
            this.invokerTypeMapper = invokerTypeMapper;
        }

        public void Dispose()
        {
        }

        public object Evaluate(InvokationContext context)
        {
            var serviceObject = context.Fields[0].ArgObject;
            var serviceType = context.Fields[0].ArgType;
            var method = serviceType.GetMethod(context.MethodName);
            var methodParams = method.GetParameters();

            var paramsList = new List<object>();

            for (int i = 0; i < methodParams.Length; i++)
            {
                var methodParam = methodParams[i];
                var param = context.Parameters[i];

                var invokeParam = invokerTypeMapper.Convert(param.ArgObject, param.ArgType, methodParam.ParameterType);

                paramsList.Add(invokeParam);
            }

            var retObject = method.Invoke(serviceObject, paramsList.ToArray());

            return invokerTypeMapper.Convert(retObject, method.ReturnType, context.ReturnType);
        }
    }
}