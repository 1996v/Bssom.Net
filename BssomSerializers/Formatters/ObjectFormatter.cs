
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Formatters;
using System.Collections.Concurrent;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;

namespace Bssom.Serializer.Formatters
{
    /// <summary>
    /// Format the real type of Object
    /// </summary>
    public sealed class ObjectFormatter : IBssomFormatter<object>
    {
        private delegate void SerializeMethod(object dynamicFormatter, ref BssomWriter writer, ref BssomSerializeContext context, object value);
        private delegate int SizeMethod(object dynamicFormatter, ref BssomSizeContext context, object value);

        public static readonly ObjectFormatter Instance = new ObjectFormatter();

        private static readonly ConcurrentDictionary<Type, SerializeMethod> SerializerDelegates = new ConcurrentDictionary<Type, SerializeMethod>();
        private static readonly ConcurrentDictionary<Type, SizeMethod> SizeDelegates = new ConcurrentDictionary<Type, SizeMethod>();

        private ObjectFormatter()
        {

        }

        public object Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            return BssomObjectFormatterHelper.DeserializeBssObject(ref reader, ref context, context.Option.IsPriorityToDeserializeObjectAsBssomValue);
        }

        public void Serialize(ref BssomWriter writer, ref BssomSerializeContext context, object value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var realType = value.GetType();
            if (realType == typeof(object))
            {
                writer.WriteArray1BuildInType(BssomType.Map2);
                BssMapObjMarshal.WriteEmptyMapObject(ref writer);
                return;
            }

            object formatter = context.Option.FormatterResolver.GetFormatterWithVerify(realType);

            if (!SerializerDelegates.TryGetValue(realType, out SerializeMethod serializerDelegate))
            {
                Type formatterType = typeof(IBssomFormatter<>).MakeGenericType(realType);
                ParameterExpression param0 = Expression.Parameter(typeof(object), "formatter");
                ParameterExpression param1 = Expression.Parameter(typeof(BssomWriter).MakeByRefType(), "writer");
                ParameterExpression param2 = Expression.Parameter(typeof(BssomSerializeContext).MakeByRefType(), "context");
                ParameterExpression param3 = Expression.Parameter(typeof(object), "value");

                MethodInfo serializeMethod = formatterType.GetRuntimeMethod(nameof(Serialize), new[] { typeof(BssomWriter).MakeByRefType(), typeof(BssomSerializeContext).MakeByRefType(), realType });

                MethodCallExpression body = Expression.Call(
                    Expression.Convert(param0, formatterType),
                    serializeMethod,
                    param1, param2,
                    realType.IsValueType ? Expression.Unbox(param3, realType) : Expression.Convert(param3, realType)
                    );

                serializerDelegate = Expression.Lambda<SerializeMethod>(body, param0, param1, param2, param3).Compile();
                SerializerDelegates.TryAdd(realType, serializerDelegate);
            }

            serializerDelegate(formatter, ref writer, ref context, value);

        }

        public int Size(ref BssomSizeContext context, object value)
        {
            if (value == null)
            {
                return BssomBinaryPrimitives.NullSize;
            }

            var realType = value.GetType();
            if (realType == typeof(object))
            {
                return BssMapObjMarshal.Empty.Length + BssomBinaryPrimitives.BuildInTypeCodeSize;
            }

            object formatter = context.Option.FormatterResolver.GetFormatterWithVerify(realType);

            if (!SizeDelegates.TryGetValue(realType, out SizeMethod sizeDelegate))
            {
                Type formatterType = typeof(IBssomFormatter<>).MakeGenericType(realType);
                ParameterExpression param0 = Expression.Parameter(typeof(object), "formatter");
                ParameterExpression param1 = Expression.Parameter(typeof(BssomSizeContext).MakeByRefType(), "context");
                ParameterExpression param2 = Expression.Parameter(typeof(object), "value");

                MethodInfo sizeMethod = formatterType.GetRuntimeMethod(nameof(Size), new[] { typeof(BssomSizeContext).MakeByRefType(), realType });

                MethodCallExpression body = Expression.Call(
                    Expression.Convert(param0, formatterType),
                    sizeMethod, param1,
                    realType.IsValueType ? Expression.Unbox(param2, realType) : Expression.Convert(param2, realType)
                    );

                sizeDelegate = Expression.Lambda<SizeMethod>(body, param0, param1, param2).Compile();
                SizeDelegates.TryAdd(realType, sizeDelegate);
            }

            return sizeDelegate(formatter, ref context, value);
        }
    }


}
namespace Bssom.Serializer
{
    internal static class RawObjectDeserializer
    {
        private delegate object DeserializeMethod(object dynamicFormatter, ref BssomReader reader, ref BssomDeserializeContext context);

        private static readonly ConcurrentDictionary<Type, DeserializeMethod> DeserializerDelegates = new ConcurrentDictionary<Type, DeserializeMethod>();

        public static object Deserialize(Type type, ref BssomReader reader, ref BssomDeserializeContext context)
        {
            object formatter = context.Option.FormatterResolver.GetFormatterWithVerify(type);

            if (!DeserializerDelegates.TryGetValue(type, out DeserializeMethod deserializerDelegate))
            {
                Type formatterType = typeof(IBssomFormatter<>).MakeGenericType(type);
                ParameterExpression param0 = Expression.Parameter(typeof(object), "formatter");
                ParameterExpression param1 = Expression.Parameter(typeof(BssomReader).MakeByRefType(), "reader");
                ParameterExpression param2 = Expression.Parameter(typeof(BssomDeserializeContext).MakeByRefType(), "context");

                MethodInfo deserializeMethod = formatterType.GetRuntimeMethod(nameof(Deserialize), new[] { typeof(BssomReader).MakeByRefType(), typeof(BssomDeserializeContext).MakeByRefType() });

                //(object)IBssomFormatter<T>.Deserialize(ref reader,option);
                var body = Expression.Convert(Expression.Call(
                    Expression.Convert(param0, formatterType),
                    deserializeMethod,
                    param1,
                    param2), typeof(object));

                deserializerDelegate = Expression.Lambda<DeserializeMethod>(body, param0, param1, param2).Compile();
                DeserializerDelegates.TryAdd(type, deserializerDelegate);
            }

            return deserializerDelegate(formatter, ref reader, ref context);
        }
    }
}
