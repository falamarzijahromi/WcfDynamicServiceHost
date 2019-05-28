using System;
using DynamicServiceHost.Host.DGenRequirements;

namespace DynamicServiceHost.Host.Abstracts
{
    public interface IHostInvoker : IDisposable
    {
        object Evaluate(HostInvokationContext context);
    }
}