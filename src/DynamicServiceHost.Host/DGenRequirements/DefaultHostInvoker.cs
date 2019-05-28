using DynamicServiceHost.Host.Abstracts;
using System.Linq;

namespace DynamicServiceHost.Host.DGenRequirements
{
    internal class DefaultHostInvoker : IHostInvoker
    {
        public void Dispose()
        {
        }

        public object Evaluate(HostInvokationContext context)
        {
            var serviceObject = context.Fields[0].ArgObject;
            var serviceType = context.Fields[0].ArgType;
            var method = serviceType.GetMethod(context.MethodName);
            var methodParams = context.Parameters.Select(param => param.ArgObject);

            return method.Invoke(serviceObject, methodParams.ToArray());
        }
    }
}