using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;

namespace Bssom.Serializer.Internal
{
    internal static class NativeDecimalGetterHelper
    {
        private readonly static Func<Decimal, DecimalBinaryBits> _func;

        static NativeDecimalGetterHelper()
        {
            try { _func = Build(); }
            catch (Exception e)
            {
                DEBUG.Throw(e);

                _func = (de) =>
                {
                    var ints = Decimal.GetBits(de);
                    return new DecimalBinaryBits(ints[0], ints[1], ints[2], ints[3]);
                };
            }
        }

        private static Func<Decimal, DecimalBinaryBits> Build()
        {
            var de = Expression.Parameter(typeof(Decimal));
            Expression low, mid, high, flags;
            try
            {
                //FRAMEWORK
                low = Expression.Field(de, typeof(Decimal).GetField("lo", BindingFlags.Instance | BindingFlags.NonPublic));
                mid = Expression.Field(de, typeof(Decimal).GetField("mid", BindingFlags.Instance | BindingFlags.NonPublic));
                high = Expression.Field(de, typeof(Decimal).GetField("hi", BindingFlags.Instance | BindingFlags.NonPublic));
                flags = Expression.Field(de, typeof(Decimal).GetField("flags", BindingFlags.Instance | BindingFlags.NonPublic));
            }
            catch
            {
                try
                {
                    low = Expression.Convert(Expression.Field(de, typeof(Decimal).GetField("Low", BindingFlags.Instance | BindingFlags.NonPublic)), typeof(int));
                    mid = Expression.Convert(Expression.Field(de, typeof(Decimal).GetField("Mid", BindingFlags.Instance | BindingFlags.NonPublic)), typeof(int));
                    high = Expression.Convert(Expression.Field(de, typeof(Decimal).GetField("High", BindingFlags.Instance | BindingFlags.NonPublic)), typeof(int));
                    flags = Expression.Field(de, typeof(Decimal).GetField("_flags", BindingFlags.Instance | BindingFlags.NonPublic));
                }
                catch (Exception ex)
                {
                    throw BssomSerializationTypeFormatterException.TypeFormatterError(typeof(decimal), ex.Message);
                }
            }

            var body = Expression.New(typeof(DecimalBinaryBits).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }), low, mid, high, flags);
            return Expression.Lambda<Func<Decimal, DecimalBinaryBits>>(body, de).Compile();
        }

        public static DecimalBinaryBits GetPack(Decimal de)
        {
            return _func(de);
        }
    }
}
