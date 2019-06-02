using DynamicWcfServiceHost.Shared.Abstracts;
using System;
using System.Collections.Generic;

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
            if (context.MethodName.Contains(nameof(IDisposable.Dispose)))
            {
                DisposeFields(context);

                return null;
            }
            else
            {
                return InvokeRelatedField(context);
            }
        }

        private object InvokeRelatedField(InvokationContext context)
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

                var invokeParam =
                    invokerTypeMapper.Convert(param.ArgObject, param.ArgType, methodParam.ParameterType);

                paramsList.Add(invokeParam);
            }

            var retObject = method.Invoke(serviceObject, paramsList.ToArray());

            if (!context.ReturnType.Equals(typeof(void)))
            {
                return invokerTypeMapper.Convert(retObject, method.ReturnType, context.ReturnType);
            }
            else
            {
                return null;
            }

        }

        private void DisposeFields(InvokationContext context)
        {
            foreach (var contextField in context.Fields)
            {
                if (contextField.ArgObject is IDisposable)
                {
                    ((IDisposable)contextField.ArgObject).Dispose();
                }
            }
        }
    }
}