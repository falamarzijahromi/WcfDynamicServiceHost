using System.Collections;
using System.Collections.Generic;

namespace DynamicServiceHost.Host.Tests.TestTypes
{
    public class SimpleDto
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as SimpleDto;
            return dto != null &&
                   Name == dto.Name &&
                   Index == dto.Index;
        }

        public override int GetHashCode()
        {
            var hashCode = -823584703;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }
    }
}