using DynamicServiceHost.Host.Abstracts;
using DynamicTypeGenerator.Abstracts;
using DynamicTypeGenerator.Invokations;
using System.Collections.Generic;

namespace DynamicServiceHost.Host.DGenRequirements
{
    internal class Evaluator<T> : IInvokationEvaluator where T : IHostInvoker
    {
        private readonly T hostEvaluator;

        public Evaluator(T hostEvaluator)
        {
            this.hostEvaluator = hostEvaluator;
        }

        public object Evaluate(InvokationContext context)
        {
            var hostInvokationContext = CreateHostInvokationContext(context);

            return hostEvaluator.Evaluate(hostInvokationContext);
        }

        private HostInvokationContext CreateHostInvokationContext(InvokationContext context)
        {
            var paramList = CreateParamList(context);
            var fieldList = CreateFieldList(context);

            return new HostInvokationContext
            {
                MethodName = context.MethodName,
                ReturnType = context.ReturnType,
                Parameters = paramList,
                Fields = fieldList,
            };
        }

        private static List<HostArgInfo> CreateFieldList(InvokationContext context)
        {
            var fieldList = new List<HostArgInfo>();

            foreach (var field in context.InjectedFields)
            {
                fieldList.Add(field);
            }

            return fieldList;
        }

        private static List<HostArgInfo> CreateParamList(InvokationContext context)
        {
            var paramList = new List<HostArgInfo>();

            foreach (var param in context.Parameters)
            {
                paramList.Add(param);
            }

            return paramList;
        }
    }
}
