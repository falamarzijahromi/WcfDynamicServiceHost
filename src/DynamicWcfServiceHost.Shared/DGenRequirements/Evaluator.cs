using DynamicTypeGenerator.Abstracts;
using DynamicWcfServiceHost.Shared.Abstracts;
using System;
using System.Collections.Generic;

namespace DynamicWcfServiceHost.Shared.DGenRequirements
{
    public class Evaluator<T> : IInvokationEvaluator where T : IInvoker
    {
        private readonly T hostEvaluator;

        public Evaluator(T hostEvaluator)
        {
            this.hostEvaluator = hostEvaluator;
        }

        public object Evaluate(DynamicTypeGenerator.Invokations.InvokationContext context)
        {
            var hostInvokationContext = CreateHostInvokationContext(context);

            var retVal = hostEvaluator.Evaluate(hostInvokationContext);

            retVal = RedifineRetVal(retVal, context.ReturnType);

            return retVal;
        }

        public static Type CreateDefaultEvaluatorType(Type invokerType)
        {
            var nonSetEvaluatorType = typeof(Evaluator<>);

            var setEvaluatorType = nonSetEvaluatorType.MakeGenericType(new[] { invokerType });

            return setEvaluatorType;
        }

        public static object CreateDefaultEvaluator(Type invokerType, IInvokerTypeMapper invokerTypeMapper)
        {
            var evaluatorType = CreateDefaultEvaluatorType(invokerType);

            var defaultInvoker = Activator.CreateInstance(invokerType, new[] { invokerTypeMapper });

            var evaluator = Activator.CreateInstance(evaluatorType, new[] { defaultInvoker });

            return evaluator;
        }

        public static Type GetInvokationEvaluatorType()
        {
            return typeof(IInvokationEvaluator);
        }

        private InvokationContext CreateHostInvokationContext(DynamicTypeGenerator.Invokations.InvokationContext context)
        {
            var paramList = CreateParamList(context);
            var fieldList = CreateFieldList(context);

            return new InvokationContext
            {
                MethodName = context.MethodName,
                ReturnType = context.ReturnType,
                Parameters = paramList,
                Fields = fieldList,
            };
        }

        private static List<ArgInfo> CreateFieldList(DynamicTypeGenerator.Invokations.InvokationContext context)
        {
            var fieldList = new List<ArgInfo>();

            foreach (var field in context.InjectedFields)
            {
                fieldList.Add(field);
            }

            return fieldList;
        }

        private static List<ArgInfo> CreateParamList(DynamicTypeGenerator.Invokations.InvokationContext context)
        {
            var paramList = new List<ArgInfo>();

            foreach (var param in context.Parameters)
            {
                paramList.Add(param);
            }

            return paramList;
        }

        private object RedifineRetVal(object retVal, Type returnType)
        {
            if (retVal == null && !returnType.Equals(typeof(void)) && !CanConvertNullTo(returnType))
            {
                return Activator.CreateInstance(returnType);
            }

            return retVal;
        }

        private bool CanConvertNullTo(Type returnType)
        {
            var isPossible = true;

            try
            {
                Convert.ChangeType(null, returnType);
            }
            catch (Exception)
            {
                isPossible = false;
            }

            return isPossible;
        }
    }
}
