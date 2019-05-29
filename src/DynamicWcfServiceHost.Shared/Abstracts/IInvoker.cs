using DynamicWcfServiceHost.Shared.DGenRequirements;
using System;

namespace DynamicWcfServiceHost.Shared.Abstracts
{
    public interface IInvoker : IDisposable
    {
        object Evaluate(InvokationContext context);
    }
}