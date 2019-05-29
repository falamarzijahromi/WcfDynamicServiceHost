using DynamicWcfServiceHost.Shared.Abstracts;
using Mapster;
using System;

namespace DynamicWcfServiceHost.Shared.DGenRequirements
{
    public class DefaultInvokerMapper : IInvokerTypeMapper
    {
        private readonly ServicePack servicePack;

        public DefaultInvokerMapper(ServicePack servicePack)
        {
            this.servicePack = servicePack;
        }

        public object Convert(object @object, Type from, Type to)
        {
            if (IsConvertable(from))
            {
                return ConvertIt(@object, from, to);
            }

            return @object;
        }

        private object ConvertIt(object @object, Type from, Type to)
        {
            var toObject = @object.Adapt(from, to);

            return toObject;
        }

        private bool IsConvertable(Type type)
        {
            return
                type != null &&
                type.IsClass &&
                !type.IsSealed &&
                type.BaseType != null &&
                !type.IsGenericType &&
                !type.IsArray;

        }
    }
}
