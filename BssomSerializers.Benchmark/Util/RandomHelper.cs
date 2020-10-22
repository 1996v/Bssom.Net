using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace BssomSerializers.Benchmark
{
    internal static class RandomHelper
    {
        static readonly char[] ASCII;
        static Random Rand = new Random(314159265);

        static RandomHelper()
        {
            var cs = new List<char>();

            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var c = (char)i;
                if (char.IsControl(c)) continue;

                cs.Add(c);
            }

            ASCII = cs.ToArray();
        }

        public static T RandomValue<T>(bool stringValueAllowEmpty = true)
        {
            return (T)RandomValue(typeof(T), stringValueAllowEmpty);
        }

        public static object RandomValue(this Type t, bool stringValueAllowEmpty = true)
        {
            if (t.IsPrimitive)
            {
                if (t == typeof(byte))
                {
                    return (byte)(Rand.Next(byte.MaxValue - byte.MinValue + 1) + byte.MinValue);
                }

                if (t == typeof(sbyte))
                {
                    return (sbyte)(Rand.Next(sbyte.MaxValue - sbyte.MinValue + 1) + sbyte.MinValue);
                }

                if (t == typeof(short))
                {
                    return (short)(Rand.Next(short.MaxValue - short.MinValue + 1) + short.MinValue);
                }

                if (t == typeof(ushort))
                {
                    return (ushort)(Rand.Next(ushort.MaxValue - ushort.MinValue + 1) + ushort.MinValue);
                }

                if (t == typeof(int))
                {
                    var bytes = new byte[4];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToInt32(bytes, 0);
                }

                if (t == typeof(uint))
                {
                    var bytes = new byte[4];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToUInt32(bytes, 0);
                }

                if (t == typeof(long))
                {
                    var bytes = new byte[8];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToInt64(bytes, 0);
                }

                if (t == typeof(ulong))
                {
                    var bytes = new byte[8];
                    Rand.NextBytes(bytes);

                    return BitConverter.ToUInt64(bytes, 0);
                }

                if (t == typeof(float))
                {
                    var bytes = new byte[4];
                    Rand.NextBytes(bytes);

                    var f = BitConverter.ToSingle(bytes, 0);
                    if (float.IsNaN(f))
                        f = (float)RandomValue<short>();
                    return f;
                }

                if (t == typeof(double))
                {
                    var bytes = new byte[8];
                    Rand.NextBytes(bytes);

                    var d = BitConverter.ToDouble(bytes, 0);
                    if (double.IsNaN(d))
                        d = (double)RandomValue<short>();
                    return d;
                }

                if (t == typeof(char))
                {
                    var roll = Rand.Next(ASCII.Length);

                    return ASCII[roll];
                }

                if (t == typeof(bool))
                {
                    return (Rand.Next(2) == 1);
                }

                throw new InvalidOperationException();
            }

            if (t == typeof(decimal))
            {
                return new decimal((int)typeof(int).RandomValue(), (int)typeof(int).RandomValue(), (int)typeof(int).RandomValue(), false, 28);
            }

            if (t == typeof(string))
            {
                int start = stringValueAllowEmpty ? 0 : 1;
                var len = Rand.Next(start, 40);
                var c = new char[len];
                for (var i = 0; i < c.Length; i++)
                {
                    c[i] = (char)typeof(char).RandomValue();
                }

                return new string(c);
            }

            if (t == typeof(DateTime))
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var bytes = new byte[4];
                Rand.NextBytes(bytes);

                var secsOffset = BitConverter.ToInt32(bytes, 0);

                var retDate = epoch.AddSeconds(secsOffset);

                return retDate;
            }

            if (t == typeof(TimeSpan))
            {
                return new TimeSpan(RandomValue<DateTime>().Ticks);
            }

            if (t == typeof(DataTable))
            {
                DataTable dt = new DataTable();
                int coluCount = Rand.Next(10, 30);
                for (int i = 0; i < coluCount; i++)
                {
                    dt.Columns.Add(RandomHelper.RandomValue<string>(false), typeof(object));
                }
                int rowCount = Rand.Next(20, 50);
                for (int i = 0; i < rowCount; i++)
                {
                    var row = new object[coluCount];
                    for (int zi = 0; zi < coluCount; zi++)
                    {
                        row[zi] = RandomHelper.RandomValue<object>();
                    }
                    dt.Rows.Add(row);
                }
                return dt;
            }

            if (t.IsNullable())
            {
                // leave it unset
                if (Rand.Next(2) == 0)
                {
                    // null!
                    return Activator.CreateInstance(t);
                }

                var underlying = Nullable.GetUnderlyingType(t);
                var val = underlying.RandomValue(stringValueAllowEmpty);

                var cons = t.GetConstructor(new[] { underlying });

                return cons.Invoke(new object[] { val });
            }

            if (t.IsEnum)
            {
                var allValues = Enum.GetValues(t);
                var ix = Rand.Next(allValues.Length);

                return allValues.GetValue(ix);
            }

            if (t.IsArray)
            {
                var valType = t.GetElementType();
                var len = Rand.Next(20, 50);
                var ret = Array.CreateInstance(valType, len);
                //var add = t.GetMethod("SetValue");
                for (var i = 0; i < len; i++)
                {
                    var elem = valType.RandomValue(stringValueAllowEmpty);
                    ret.SetValue(elem, i);
                }

                return ret;
            }

            if (t.IsGenericType)
            {
                var defind = t.GetGenericTypeDefinition();
                if (defind == typeof(HashSet<>))
                {
                    var valType = t.GetGenericArguments()[0];
                    var ret = Activator.CreateInstance(t);
                    var add = t.GetMethod("Add");
                    var contains = t.GetMethod("Contains");

                    var len = Rand.Next(20, 50);
                    for (var i = 0; i < len; i++)
                    {
                        var elem = valType.RandomValue(stringValueAllowEmpty);
                        while (elem == null || (bool)contains.Invoke(ret, new object[] { elem }))
                            elem = valType.RandomValue(stringValueAllowEmpty);
                        add.Invoke(ret, new object[] { elem });
                    }

                    return ret;
                }
                if (defind == typeof(Dictionary<,>))
                {
                    var keyType = t.GetGenericArguments()[0];
                    var valType = t.GetGenericArguments()[1];
                    var ret = Activator.CreateInstance(t);
                    var add = t.GetMethod("Add");
                    var contains = t.GetMethod("ContainsKey");

                    var len = Rand.Next(20, 50);
                    if (keyType == typeof(Boolean))
                        len = 2;
                    for (var i = 0; i < len; i++)
                    {
                        var val = valType.RandomValue(stringValueAllowEmpty);
                        var key = keyType.RandomValue(stringValueAllowEmpty);
                        while (key == null || (bool)contains.Invoke(ret, new object[] { key }))
                            key = keyType.RandomValue(stringValueAllowEmpty);
                        add.Invoke(ret, new object[] { key, val });
                    }

                    return ret;
                }
                if (defind == typeof(List<>))
                {
                    var valType = t.GetGenericArguments()[0];
                    var ret = Activator.CreateInstance(t);
                    var add = t.GetMethod("Add");

                    var len = Rand.Next(20, 50);
                    for (var i = 0; i < len; i++)
                    {
                        var elem = valType.RandomValue(stringValueAllowEmpty);
                        add.Invoke(ret, new object[] { elem });
                    }

                    return ret;
                }
                if (defind == typeof(ArraySegment<>))
                {
                    var valType = t.GetGenericArguments()[0];
                    var ary = valType.MakeArrayType().RandomValue(stringValueAllowEmpty);
                    var lenT = ary.GetType().GetProperty("Length");
                    var offset = Rand.Next(0, (int)lenT.GetValue(ary) - 1);
                    var len = (int)lenT.GetValue(ary) - offset;

                    return Activator.CreateInstance(t, ary, offset, len);
                }
            }

            if (t == typeof(Guid))
                return Guid.NewGuid();

            if (t == typeof(object))
            {
                var code = Rand.Next(0, 9);
                switch (code)
                {
                    case 0:
                        return RandomValue<int>();
                    case 1:
                        return RandomValue<long>();
                    case 2:
                        return RandomValue<Char>();
                    case 3:
                        return RandomValue<DateTime>();
                    case 4:
                        return RandomValue<string>(stringValueAllowEmpty);
                    case 5:
                        return RandomValue<Guid>();
                    case 6:
                        return RandomValue<decimal>();
                    case 7:
                        return RandomValue<double>();
                    case 8:
                        return RandomValue<float>();
                    default:
                        return RandomValue<short>();
                }
            }

            //model
            var retObj = Activator.CreateInstance(t);
            foreach (var p in t.GetFields())
            {
                //if (Rand.Next(5) == 0) continue;

                var fieldType = p.FieldType;

                p.SetValue(retObj, fieldType.RandomValue(stringValueAllowEmpty));
            }

            foreach (var p in t.GetProperties())
            {
                //if (Rand.Next(5) == 0) continue;
                if (p.CanWrite && p.CanRead)
                {
                    var fieldType = p.PropertyType;

                    p.SetValue(retObj, fieldType.RandomValue(stringValueAllowEmpty));
                }
            }

            return retObj;
        }


        public static object RandomValueWithOutStringEmpty(Type t)
        {
            return RandomValue(t, false);
        }

        public static T RandomValueWithOutStringEmpty<T>()
        {
            return RandomValue<T>(false);
        }


        private static bool IsNullable(this Type t)
        {
            return Nullable.GetUnderlyingType(t) != null;
        }
    }

    public static class RandomFieldName
    {
        private static Random random = new Random();
        private const string name = "abcdefghijklmnopqrstuvwxyz0123456789";

        public static string Random(int length)
        {
            var cArray = new char[length];
            for (var i = 0; i < length; i++)
            {
                cArray[i] = name[random.Next(name.Length)];
            }

            return new string(cArray);
        }
    }

    public class EntryR {
        public string Name;
        public int FieldNum;
        public int TypeCategory;
        public int FieldNameLength;

        public EntryR(string name, int fieldNum, int typeCategory, int fieldNameLength)
        {
            Name = name;
            FieldNum = fieldNum;
            TypeCategory = typeCategory;
            FieldNameLength = fieldNameLength;
        }
    }
}
