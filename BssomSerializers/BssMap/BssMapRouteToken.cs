//using System.Runtime.CompilerServices;
using BssomSerializers.Binary;
using BssomSerializers.BssMap.KeyResolvers;
using BssomSerializers.Internal;
using BssomSerializers.BssomBuffer;
using System.Runtime.CompilerServices;

namespace BssomSerializers.BssMap
{
    internal enum BssMapRouteToken : byte
    {
        EqualNext1 = 1,
        EqualNext2 = 2,
        EqualNext3 = 3,
        EqualNext4 = 4,
        EqualNext5 = 5,
        EqualNext6 = 6,
        EqualNext7 = 7,
        EqualNext8 = 8,
        EqualNextN = 9,

        EqualLast1 = 11,
        EqualLast2 = 12,
        EqualLast3 = 13,
        EqualLast4 = 14,
        EqualLast5 = 15,
        EqualLast6 = 16,
        EqualLast7 = 17,
        EqualLast8 = 18,
        EqualLastN = 19,

        LessThen1 = 21,
        LessThen2 = 22,
        LessThen3 = 23,
        LessThen4 = 24,
        LessThen5 = 25,
        LessThen6 = 26,
        LessThen7 = 27,
        LessThen8 = 28,

        LessElse = 30,
        HasChildren = 31,
        NoChildren = 32,
    }
    internal static class BssMapRouteTokenHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetEqualNextOrLastByteCount(BssMapRouteToken equalNext)
        {
            DEBUG.Assert(equalNext >= BssMapRouteToken.EqualNext1 && equalNext <= BssMapRouteToken.EqualNext8 ||
                equalNext >= BssMapRouteToken.EqualLast1 && equalNext <= BssMapRouteToken.EqualLast8);

            byte t = (byte)equalNext;
            if (t > 10)
                return (byte)(t - 10);
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetLessThenByteCount(BssMapRouteToken equalNext)
        {
            DEBUG.Assert(equalNext >= BssMapRouteToken.LessThen1 && equalNext <= BssMapRouteToken.LessThen8);

            byte t = (byte)equalNext;
            if (t > 20)
                return (byte)(t - 20);
            return t;
        }
    }
}
