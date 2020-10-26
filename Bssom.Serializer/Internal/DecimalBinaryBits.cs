using Bssom.Serializer.Binary;
using System.Runtime.CompilerServices;
namespace Bssom.Serializer.Internal
{
    internal struct DecimalBinaryBits
    {
        public int Low;
        public int Mid;
        public int High;
        public int Flags;

        public const int Size = 16;

        public DecimalBinaryBits(int low, int mid, int high, int flags)
        {
            Low = low;
            Mid = mid;
            High = high;
            Flags = flags;
        }

        public void Write(ref byte refb)
        {
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref refb, Low);
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref Unsafe.Add(ref refb, 4), Mid);
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref Unsafe.Add(ref refb, 8), High);
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref Unsafe.Add(ref refb, 12), Flags);
        }

        public static decimal Read(ref byte refb)
        {
            int low = BssomBinaryPrimitives.ReadInt32LittleEndian(ref refb);
            int mid = BssomBinaryPrimitives.ReadInt32LittleEndian(ref Unsafe.Add(ref refb, 4));
            int high = BssomBinaryPrimitives.ReadInt32LittleEndian(ref Unsafe.Add(ref refb, 8));
            int flags = BssomBinaryPrimitives.ReadInt32LittleEndian(ref Unsafe.Add(ref refb, 12));

            bool sign = (flags & 0x80000000) != 0;
            byte scale = (byte)((flags >> 16) & 0x7F);
            return new decimal(low, mid, high, sign, scale);
        }
    }
}