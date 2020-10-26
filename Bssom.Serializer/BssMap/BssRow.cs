//using System.Runtime.CompilerServices;

using Bssom.Serializer.Internal;

namespace Bssom.Serializer.BssMap
{
    internal class BssRow<TValue>
    {
        public IMapKeySegment Key;
        public TValue Value;
        public byte KeyType;
        public bool KeyIsNativeType;

        public BssRow(IMapKeySegment key, TValue value, byte keyType, bool keyIsNativeType)
        {
            Key = key;
            Value = value;
            KeyType = keyType;
            KeyIsNativeType = keyIsNativeType;
        }
    }
}
