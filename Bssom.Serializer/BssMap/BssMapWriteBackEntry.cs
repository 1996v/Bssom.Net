//using System.Runtime.CompilerServices;
using System.Reflection;

namespace Bssom.Serializer.BssMap
{
    internal struct BssMapWriteBackEntry
    {
        public static FieldInfo _ValueOffset = typeof(BssMapWriteBackEntry).GetField(nameof(ValueOffset));
        public static FieldInfo _MapOffset = typeof(BssMapWriteBackEntry).GetField(nameof(MapOffset));

        public int MapOffset;
        public uint ValueOffset;
    }
}
