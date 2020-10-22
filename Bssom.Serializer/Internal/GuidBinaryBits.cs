using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Resolver;
using System.Linq.Expressions;
using Bssom.Serializer.Binary;

namespace Bssom.Serializer.Internal
{
    internal struct GuidBinaryBits
    {
        public const int Size = 16;

        public int _a;   // Do not rename 
        public short _b; // Do not rename 
        public short _c; // Do not rename 
        public byte _d;  // Do not rename 
        public byte _e;  // Do not rename 
        public byte _f;  // Do not rename 
        public byte _g;  // Do not rename 
        public byte _h;  // Do not rename 
        public byte _i;  // Do not rename 
        public byte _j;  // Do not rename 
        public byte _k;  // Do not rename 

        public GuidBinaryBits(ref byte refb)
        {
            _a = BssomBinaryPrimitives.ReadInt32LittleEndian(ref refb);
            _b = BssomBinaryPrimitives.ReadInt16LittleEndian(ref Unsafe.Add(ref refb, 4));
            _c = BssomBinaryPrimitives.ReadInt16LittleEndian(ref Unsafe.Add(ref refb, 6));
            _d = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 8));
            _e = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 9));
            _f = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 10));
            _g = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 11));
            _h = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 12));
            _i = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 13));
            _j = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 14));
            _k = BssomBinaryPrimitives.ReadUInt8(ref Unsafe.Add(ref refb, 15));
        }

        public GuidBinaryBits(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
            _e = e;
            _f = f;
            _g = g;
            _h = h;
            _i = i;
            _j = j;
            _k = k;
        }

        private static readonly Func<Guid, GuidBinaryBits> GetGuidBinaryBitsFunc;

        static GuidBinaryBits()
        {
            ParameterExpression guid = Expression.Parameter(typeof(Guid));
            GetGuidBinaryBitsFunc = LambdaExpression.Lambda<Func<Guid, GuidBinaryBits>>(
                Expression.New(typeof(GuidBinaryBits).GetConstructor(new Type[] { typeof(int), typeof(short), typeof(short), typeof(byte), typeof(byte), typeof(byte), typeof(byte), typeof(byte), typeof(byte), typeof(byte), typeof(byte) }),
                   Expression.Field(guid, nameof(_a)), Expression.Field(guid, nameof(_b)), Expression.Field(guid, nameof(_c)), Expression.Field(guid, nameof(_d)), Expression.Field(guid, nameof(_e)), Expression.Field(guid, nameof(_f)), Expression.Field(guid, nameof(_g)), Expression.Field(guid, nameof(_h)), Expression.Field(guid, nameof(_i)), Expression.Field(guid, nameof(_j)), Expression.Field(guid, nameof(_k))),
                guid).Compile();
        }

        public static GuidBinaryBits GetGuidBinaryBits(Guid guid)
        {
            return GetGuidBinaryBitsFunc(guid);
        }

        public void Write(ref byte refb)
        {
            BssomBinaryPrimitives.WriteInt32LittleEndian(ref refb, _a);
            BssomBinaryPrimitives.WriteInt16LittleEndian(ref Unsafe.Add(ref refb, 4), _b);
            BssomBinaryPrimitives.WriteInt16LittleEndian(ref Unsafe.Add(ref refb, 6), _c);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 8), _d);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 9), _e);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 10), _f);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 11), _g);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 12), _h);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 13), _i);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 14), _j);
            BssomBinaryPrimitives.WriteUInt8(ref Unsafe.Add(ref refb, 15), _k);
        }

        public Guid ToGuid()
        {
            return new Guid(_a, _b, _c, _d, _e, _f, _g, _h, _i, _j, _k);
        }
    }
}
