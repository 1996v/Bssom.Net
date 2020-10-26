//using System.Runtime.CompilerServices;

namespace Bssom.Serializer.BssMap
{
    internal struct BssMapHead
    {
        public int DataLength;
        public int ElementCount;
        public int MaxDepth;
        public int RouteLength;

        public static BssMapHead Read(ref BssomReader reader)
        {
            BssMapHead header = new BssMapHead();
            header.DataLength = reader.ReadVariableNumber();
            header.ElementCount = reader.ReadVariableNumber();
            header.MaxDepth = reader.ReadVariableNumber();
            header.RouteLength = reader.ReadVariableNumber();
            return header;
        }
    }
}
