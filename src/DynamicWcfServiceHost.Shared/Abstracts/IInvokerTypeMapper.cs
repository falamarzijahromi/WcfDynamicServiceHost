using System;

namespace DynamicWcfServiceHost.Shared.Abstracts
{
    public interface IInvokerTypeMapper
    {
        object Convert(object @object, Type from, Type to);
    }
}