//using System.Runtime.CompilerServices;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Internal
{
    internal static class TypeUtils
    {
        public static IEnumerable<KeyValuePair<object, object>> GetGetEnumerator(this IDictionary value)
        {
            foreach (DictionaryEntry item in value)
            {
                yield return new KeyValuePair<object, object>(item.Key, item.Value);
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> ToIEnumerable(this NameValueCollection value)
        {
            foreach (object item in value)
            {
                yield return new KeyValuePair<string, string>((string)item, value[(string)item]);
            }
        }
        public static IEnumerable<KeyValuePair<string, string>> ToIEnumerable(this StringDictionary value)
        {
            foreach (DictionaryEntry item in value)
            {
                yield return new KeyValuePair<string, string>((string)item.Key, (string)item.Value);
            }
        }

        public static IEnumerable<KeyValuePair<string, object>> ToIEnumerable(this DataRow value)
        {
            foreach (DataColumn column in value.Table.Columns)
            {
                yield return new KeyValuePair<string, object>(column.ColumnName, value[column]);
            }
        }

        public static int GetIDictionaryCount<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> value)
        {
            DEBUG.Assert(value is IDictionary<TKey, TValue> || value is IReadOnlyDictionary<TKey, TValue>);

            if (value is IDictionary<TKey, TValue> idic)
            {
                return idic.Count;
            }
            else
            {
                return ((IReadOnlyDictionary<TKey, TValue>)value).Count;
            }
        }
        public static bool TryGetICollectionCount<T>(this IEnumerable<T> value, out int count)
        {
            if (value is ICollection<T> idic)
            {
                count = idic.Count;
                return true;
            }
            else if (value is IReadOnlyCollection<T> irdic)
            {
                count = irdic.Count;
                return true;
            }

            count = 0;
            return false;
        }

        public static bool TryGetIDictionaryCount<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> value, out int count)
        {
            if (value is IDictionary<TKey, TValue> idic)
            {
                count = idic.Count;
                return true;
            }
            else if (value is IReadOnlyDictionary<TKey, TValue> irdic)
            {
                count = irdic.Count;
                return true;
            }

            count = -1;
            return false;
        }

        public static ConstructorInfo GetDefaultNoArgCtorOrAppointTypeCtor(this Type type, Type ctorParaTypes = null)
        {
            foreach (ConstructorInfo ctor in type.GetConstructors())
            {
                ParameterInfo[] ctorParas = ctor.GetParameters();
                if (ctorParas.Length == 0)
                {
                    return ctor;//no args
                }

                if (ctorParaTypes != null && ctorParas.Length == 1)
                {
                    if (ctorParas[0].ParameterType == ctorParaTypes)
                    {
                        return ctor;
                    }
                }
            }
            return null;
        }

        public static ConstructorInfo GetAppointTypeCtor(this Type type, Type ctorParaType)
        {
            foreach (ConstructorInfo ctor in type.GetConstructors())
            {
                ParameterInfo[] ctorParas = ctor.GetParameters();
                if (ctorParas.Length == 1)
                {
                    if (ctorParas[0].ParameterType == ctorParaType)
                    {
                        return ctor;
                    }
                }
            }
            return null;
        }

        public static List<MemberInfo> GetAllInterfaceMembers(this Type type)
        {
            Stack<Type> pending = new Stack<Type>();
            pending.Push(type);
            List<MemberInfo> ret = new List<MemberInfo>();
            while (pending.Count > 0)
            {
                Type current = pending.Pop();

                ret.AddRange(current.GetMembers());

                if (current.BaseType != null)
                {
                    pending.Push(current.BaseType);
                }

                foreach (Type x in current.GetInterfaces())
                {
                    pending.Push(x);
                }
            }
            return ret;
        }

        public static IEnumerable<KeyValuePair<string, object>> GetPublicMembersWithDynamicObject(this object value)
        {
            DEBUG.Assert(value != null);
            Type t = value.GetType();
            foreach (FieldInfo p in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                yield return new KeyValuePair<string, object>(p.Name, p.GetValue(value));
            }
            foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.GetIndexParameters().Length == 0 && p.CanRead && p.CanWrite)
                {
                    yield return new KeyValuePair<string, object>(p.Name, p.GetValue(value));
                }
            }
        }

        public static bool IsAnonymousType(this Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
