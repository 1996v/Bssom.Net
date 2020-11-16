//using System.Runtime.CompilerServices;

using System;

namespace Bssom.Serializer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public int Index { get; }

        public KeyAttribute(int index)
        {
            if (index < 0)
                throw new ArgumentException(nameof(index));

            Index = index;
        }
    }
}
