//using System.Runtime.CompilerServices;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using System.Reflection;

namespace BssomSerializers.BssMap
{
    internal struct BssMapWriteBackEntry
    {
        public static FieldInfo _ValueOffset = typeof(BssMapWriteBackEntry).GetField(nameof(ValueOffset));
        public static FieldInfo _MapOffset = typeof(BssMapWriteBackEntry).GetField(nameof(MapOffset));

        public int MapOffset;
        public uint ValueOffset;
    }
}
