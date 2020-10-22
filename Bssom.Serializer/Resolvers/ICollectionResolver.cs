//using System.Runtime.CompilerServices;

using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Resolver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.Resolver
{
    /// <summary>
    /// <para>获取和生成具有IColloction行为的类型的<see cref="IBssomFormatter"/></para>
    /// <para>Get and generate the type with IColloction behavior <see cref="IBssomFormatter"/></para>
    /// </summary>
    public sealed class ICollectionResolver : IFormatterResolver
    {
        internal const string ModuleName = "Bssom.Serializer.Resolvers.IColloctionResolver";
        internal static readonly DynamicFormatterAssembly DynamicAssembly;

        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly ICollectionResolver Instance;

        static ICollectionResolver()
        {
            Instance = new ICollectionResolver();
            DynamicAssembly = new DynamicFormatterAssembly(ModuleName);
        }

        public IBssomFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IBssomFormatter<T> Formatter;

            static FormatterCache()
            {
                Type t = typeof(T);

                if (Array1ResolverGetFormatterHelper.TryGetFormatter(t, out var formatter))
                {
                    Formatter = (IBssomFormatter<T>)formatter;
                    return;
                }

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ArraySegment<>))
                {
                    Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(ArraySegmentFormatter<>).MakeGenericType(t.GetGenericArguments()));
                    return;
                }

                if (TypeIsArray(t, out int rank, out Type elementType))
                {
                    if (rank == 1)
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(OneDimensionalArrayFormatter<>).MakeGenericType(elementType));
                    }
                    else if (rank == 2)
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(TwoDimensionalArrayFormatter<>).MakeGenericType(elementType));
                    }
                    else if (rank == 3)
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(ThreeDimensionalArrayFormatter<>).MakeGenericType(elementType));
                    }
                    else if (rank == 4)
                    {
                        Formatter = (IBssomFormatter<T>)Activator.CreateInstance(typeof(FourDimensionalArrayFormatter<>).MakeGenericType(elementType));
                    }
                    else
                    {
                        throw BssomSerializationTypeFormatterException.UnsupportedType(t);
                    }
                    return;
                }

                if (TypeIsCollection(t,
                    out ConstructorInfo constructor,
                    out Type itemType,
                    out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList))
                {
                    TypeInfo buildType;
                    if (t.IsInterface)
                        buildType = ICollectionFormatterTypeBuilder.BuildICollectionInterfaceType(DynamicAssembly, t, itemType);
                    else
                        buildType = ICollectionFormatterTypeBuilder.BuildICollectionImplementationType(DynamicAssembly, t, constructor,  itemType, isImplGenerIList, IsImplIList, isImplGenerICollec, isImplIReadOnlyList);
                    Formatter = (IBssomFormatter<T>)Activator.CreateInstance(buildType);
                }
            }
        }


        internal static bool TypeIsCollection(Type t, out ConstructorInfo constructor,  out Type itemType, out bool isImplGenerIList, out bool IsImplIList, out bool isImplGenerICollec, out bool isImplIReadOnlyList)
        {
            constructor = null;
            itemType = null;
            IsImplIList = false;
            isImplGenerIList = false;
            isImplGenerICollec = false;
            isImplIReadOnlyList = false;

            if (t.IsInterface)
            {
                if (t == typeof(IEnumerable) || t == typeof(ICollection) || t == typeof(IList))
                {
                    itemType = typeof(object);
                    if (t == typeof(IList))
                        IsImplIList = true;

                    return true;
                }

                if (t.IsGenericType)
                {
                    var genericType = t.GetGenericTypeDefinition();
                    if (genericType == typeof(IEnumerable<>) || genericType == typeof(IList<>) || genericType == typeof(ICollection<>) || genericType == typeof(ISet<>) || genericType == typeof(IReadOnlyList<>) || genericType == typeof(IReadOnlyCollection<>))
                    {
                        if (genericType == typeof(IList<>))
                        {
                            isImplGenerIList = true;
                            isImplGenerICollec = true;
                        }
                        else if (genericType == typeof(ICollection<>) || genericType == typeof(ISet<>))
                        {
                            isImplGenerICollec = true;
                        }
                        else if (genericType == typeof(IReadOnlyList<>))
                        {
                            isImplIReadOnlyList = true;
                        }

                        itemType = t.GetGenericArguments()[0];
                        return true;
                    }
                }

                return false;
            }

            if (t.IsGenericType)
            {
                var genericType = t.GetGenericTypeDefinition();
                if (genericType == typeof(List<>))
                {
                    itemType = t.GetGenericArguments()[0];
                    isImplGenerIList = true;
                    IsImplIList = true;
                    isImplGenerICollec = true;
                    isImplIReadOnlyList = true;
                    constructor = t.GetConstructor(new Type[] { typeof(int) });
                    return true;
                }
            }

            bool isImplGenerIEnumerable = false;
            bool isImplICollection = false;
            Type generIEnumerableItemType = null;
            Type generIListItemType = null;
            Type generICollectionItemType = null;

            var intserfaces = t.GetInterfaces();
            foreach (var item in intserfaces)
            {
                if (item.IsGenericType)
                {
                    var genericTypeDefinition = item.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof(IEnumerable<>))
                    {
                        isImplGenerIEnumerable = true;
                        generIEnumerableItemType = item.GetGenericArguments()[0];
                    }
                    else if (genericTypeDefinition == typeof(ICollection<>))
                    {
                        isImplGenerICollec = true;
                        generICollectionItemType = item.GetGenericArguments()[0];
                    }
                    else if (genericTypeDefinition == typeof(IList<>))
                    {
                        isImplGenerIList = true;
                        generIListItemType = item.GetGenericArguments()[0];
                    }
                }
                else if (item == typeof(ICollection))
                    isImplICollection = true;
                else if (item == typeof(IList))
                    IsImplIList = true;
            }

            if (isImplGenerIList)
            {
                if (TryGetConstructorInfo(t, generIListItemType, true, out constructor))
                {
                    itemType = generIListItemType;
                    return true;
                }
            }

            if (isImplGenerICollec)
            {
                if (TryGetConstructorInfo(t, generICollectionItemType, true, out constructor))
                {
                    itemType = generICollectionItemType;
                    return true;
                }
            }

            if (isImplGenerIEnumerable && isImplICollection)
            {
                if (TryGetConstructorInfo(t, generIEnumerableItemType, false, out constructor))
                {
                    itemType = generIEnumerableItemType;
                    return true;
                }
            }

            if (IsImplIList)
            {
                if (TryGetConstructorInfo(t, typeof(object), true, out constructor))
                {
                    itemType = typeof(object);
                    return true;
                }
            }

            if (isImplICollection)
            {
                if (TryGetConstructorInfo(t, typeof(object), false, out constructor))
                {
                    itemType = typeof(object);
                    return true;
                }
            }

            return false;
        }

        private static bool TypeIsArray(Type t, out int rank, out Type elementType)
        {
            if (t.IsArray)
            {
                rank = t.GetArrayRank();
                elementType = t.GetElementType();
                return true;
            }
            rank = 0;
            elementType = null;
            return false;
        }

        private static bool TryGetConstructorInfo(Type targetType, Type itemType, bool isFindEmptyCtor, out ConstructorInfo constructor)
        {
            constructor = null;
            foreach (var item in targetType.GetConstructors())
            {
                var paras = item.GetParameters();

                if (isFindEmptyCtor)
                {
                    if (paras.Length == 0)
                    {
                        constructor = item;
                        return true;
                    }
                }

                if (constructor != null)
                    continue;

                if (paras.Length == 1)
                {
                    var ctorArgType = paras[0].ParameterType;
                    if (targetType == ctorArgType)
                        continue;

                    if (
                        (TypeIsArray(ctorArgType, out int rank, out Type eleType) &&
                        rank == 1 && eleType == itemType
                        ) ||
                       (TypeIsCollection(ctorArgType, out var a,  out eleType, out var c, out var d, out var e, out var f) &&
                       eleType == itemType)
                        )
                    {
                        constructor = item;
                        if (!isFindEmptyCtor)
                            return true;
                    }
                }
            }
            return constructor != null;
        }
    }
}

namespace Bssom.Serializer.Internal
{
    internal static class Array1ResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, IBssomFormatter> FormatterMap = new Dictionary<Type, IBssomFormatter>()
        {
            { typeof(Int16[]), Int16ArrayFormatter.Instance },
            { typeof(Int32[]), Int32ArrayFormatter.Instance },
            { typeof(Int64[]), Int64ArrayFormatter.Instance },
            { typeof(UInt16[]), UInt16ArrayFormatter.Instance },
            { typeof(UInt32[]), UInt32ArrayFormatter.Instance },
            { typeof(UInt64[]), UInt64ArrayFormatter.Instance },
            { typeof(Single[]), Float32ArrayFormatter.Instance },
            { typeof(Double[]), Float64ArrayFormatter.Instance },
            { typeof(bool[]), BooleanArrayFormatter.Instance },
            { typeof(byte[]), UInt8ArrayFormatter.Instance },//special
            { typeof(sbyte[]), Int8ArrayFormatter.Instance },
            { typeof(char[]), CharArrayFormatter.Instance },
            { typeof(DateTime[]), DateTimeArrayFormatter.Instance },
            { typeof(Decimal[]), DecimalArrayFormatter.Instance },
            { typeof(Guid[]),GuidArrayFormatter.Instance },

            { typeof(List<Int16>), Int16ListFormatter.Instance },
            { typeof(List<Int32>), Int32ListFormatter.Instance },
            { typeof(List<Int64>), Int64ListFormatter.Instance },
            { typeof(List<UInt16>), UInt16ListFormatter.Instance },
            { typeof(List<UInt32>), UInt32ListFormatter.Instance },
            { typeof(List<UInt64>), UInt64ListFormatter.Instance },
            { typeof(List<Single>), Float32ListFormatter.Instance },
            { typeof(List<Double>), Float64ListFormatter.Instance },
            { typeof(List<bool>), BooleanListFormatter.Instance },
            { typeof(List<byte>), UInt8ListFormatter.Instance },//special
            { typeof(List<sbyte>), Int8ListFormatter.Instance },
            { typeof(List<char>), CharListFormatter.Instance },
            { typeof(List<DateTime>), DateTimeListFormatter.Instance },
            { typeof(List<Decimal>), DecimalListFormatter.Instance },
            { typeof(List<Guid>),GuidListFormatter.Instance },

            { typeof(ArraySegment<bool>), BooleanArraySegmentFormatter.Instance },
            { typeof(ArraySegment<byte>), UInt8ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<sbyte>), Int8ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Int16>), Int16ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<UInt16>), UInt16ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Char>), CharArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Int32>), Int32ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<UInt32>),UInt32ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Int64>), Int64ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<UInt64>), UInt64ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Single>), Float32ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Double>), Float64ArraySegmentFormatter.Instance },
            { typeof(ArraySegment<DateTime>), DateTimeArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Decimal>), DecimalArraySegmentFormatter.Instance },
            { typeof(ArraySegment<Guid>),GuidArraySegmentFormatter.Instance },
        };

        internal static bool TryGetFormatter(Type t, out IBssomFormatter formatter)
        {
            if (FormatterMap.TryGetValue(t, out formatter))
            {
                return true;
            }

            return false;
        }

        internal static Type GetListFormatterType(Type itemType)
        {
            var listT = typeof(List<>).MakeGenericType(itemType);
            return FormatterMap[listT].GetType();
        }
    }

    internal static class ICollectionFormatterTypeBuilder
    {
        public static TypeInfo BuildICollectionInterfaceType(DynamicFormatterAssembly assembly, Type type, Type elementType)
        {
            TypeBuilder typeBuilder = assembly.DefineCollectionFormatterType(type, elementType);

            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);

            if (type.IsGenericType == false)
            {
                DEBUG.Assert(type == typeof(IEnumerable) || type == typeof(IList) || type == typeof(ICollection));
                //itemType is Object, Array2
                if (type == typeof(IList))
                    TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeIList), BindingFlags.Public | BindingFlags.Static));
                else
                    TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeIEnumerable), BindingFlags.Public | BindingFlags.Static));

                TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.DeserializeList), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(object)));

                TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SizeIEnumerable), BindingFlags.Public | BindingFlags.Static));
            }
            else
            {
                Type genericTypeDefine = type.GetGenericTypeDefinition();
                DEBUG.Assert(genericTypeDefine == typeof(IEnumerable<>) || genericTypeDefine == typeof(IList<>) || genericTypeDefine == typeof(ICollection<>) || genericTypeDefine == typeof(ISet<>) || genericTypeDefine == typeof(IReadOnlyList<>) || genericTypeDefine == typeof(IReadOnlyCollection<>));

                if (Array1FormatterHelper.IsArray1Type(elementType))
                {
                    //Array1
                    TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array1FormatterHelper).GetMethod(nameof(Array1FormatterHelper.SerializeIEnumerable), new Type[] { typeof(BssomWriter).MakeByRefType(), typeof(BssomSerializeContext).MakeByRefType(), typeof(IEnumerable<>).MakeGenericType(elementType) }));

                    if (genericTypeDefine == typeof(ISet<>))
                        TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, typeof(Array1FormatterHelper).GetMethod(Array1FormatterHelper.DeserializeSetPrefix + elementType.Name, BindingFlags.Public | BindingFlags.Static));
                    else
                    {
                        var listFormatterType = Array1ResolverGetFormatterHelper.GetListFormatterType(elementType);
                        var field = listFormatterType.GetField(nameof(DateTimeListFormatter.Instance), BindingFlags.Static | BindingFlags.Public);
                        var method = listFormatterType.GetMethod(nameof(DateTimeListFormatter.Deserialize));
                        TypeBuildHelper.CallOneStaticFieldMethodInDeserialize(deserializeMethod, field, method);
                    }

                    TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(Array1FormatterHelper).GetMethod(nameof(Array1FormatterHelper.SizeIEnumerable),new Type[] { typeof(BssomSizeContext).MakeByRefType(), typeof(IEnumerable<>).MakeGenericType(elementType) }));
                }
                else
                {
                    if (genericTypeDefine == typeof(IList<>) || genericTypeDefine == typeof(IReadOnlyList<>))
                        TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeGenerIList), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(elementType));
                    else
                        TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeGenericIEnumerable), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(elementType));

                    if (genericTypeDefine == typeof(ISet<>))
                        TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.DeserializeSet), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(elementType));
                    else
                        TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.DeserializeList), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(elementType));

                    TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SizeGenericIEnumerable), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(elementType));
                }
            }

            return typeBuilder.CreateTypeInfo();
        }

        public static TypeInfo BuildICollectionImplementationType(DynamicFormatterAssembly assembly, Type type, ConstructorInfo constructor,
                     Type itemType,
                     bool isImplGenerIList, bool IsImplIList, bool isImplGenerICollec, bool isImplIReadOnlyList)
        {
            TypeBuilder typeBuilder = assembly.DefineFormatterType(type);

            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);
            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            if (itemType == typeof(object))
            {
                //itemType is Object, Array2
                if (IsImplIList)
                    TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeIList), BindingFlags.Public | BindingFlags.Static));
                else
                    TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeIEnumerable), BindingFlags.Public | BindingFlags.Static));

                TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SizeIEnumerable), BindingFlags.Public | BindingFlags.Static));
            }
            else
            {
                if (Array1FormatterHelper.IsArray1Type(itemType))
                {
                    //Array1
                    TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array1FormatterHelper).GetMethod(nameof(Array1FormatterHelper.SerializeIEnumerable), new Type[] { typeof(BssomWriter).MakeByRefType(), typeof(BssomSerializeContext).MakeByRefType(), typeof(IEnumerable<>).MakeGenericType(itemType) }));

                    TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(Array1FormatterHelper).GetMethod(nameof(Array1FormatterHelper.SizeIEnumerable), new Type[] { typeof(BssomSizeContext).MakeByRefType(),typeof(IEnumerable<>).MakeGenericType(itemType) }));
                }
                else
                {
                    if (isImplGenerIList || isImplIReadOnlyList)
                        TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeGenerIList), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(itemType));
                    else
                        TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SerializeGenericIEnumerable), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(itemType));

                    TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.SizeGenericIEnumerable), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(itemType));
                }
            }

            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            var args = constructor.GetParameters();
            if (args.Length == 1 && args[0].ParameterType != typeof(int))
            {
                //new T(IEnumerable t)
                Type dynamicCacheType = typeof(CollectionDynamicDelegateCache<>).MakeGenericType(type);
                var methodinfo = dynamicCacheType.GetMethod(nameof(CollectionDynamicDelegateCache<int>.GenerateInjectCtor));
                methodinfo.Invoke(null, new object[] { constructor, args[0].ParameterType });
                TypeBuildHelper.CallDeserializeDelegate(deserializeMethod, type, dynamicCacheType.GetField(nameof(CollectionDynamicDelegateCache<int>.Deserialize), BindingFlags.Public | BindingFlags.Static));
            }
            else
            {
                if (itemType == typeof(DateTime))//DateTime需要特殊处理，因为要处理Standrand和Native
                {
                    var dtCollBuilder = typeof(DateTimeCollectionDeserializeBuilder<>).MakeGenericType(type);
                    var methodinfo = dtCollBuilder.GetMethod(nameof(DateTimeCollectionDeserializeBuilder<ICollection<DateTime>>.ConstructorInit));
                    methodinfo.Invoke(null, new object[] { constructor });

                    TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, dtCollBuilder.GetMethod(nameof(DateTimeCollectionDeserializeBuilder<ICollection<DateTime>>.Deserialize)));
                }
                else
                {
                    Type dynamicCacheType = typeof(CollectionDynamicDelegateCache<>).MakeGenericType(type);
                    if (args.Length == 0)
                    {
                        var methodinfo = dynamicCacheType.GetMethod(nameof(CollectionDynamicDelegateCache<int>.GenerateDeserializeWithEmptyCtor));
                        methodinfo.Invoke(null, new object[] { constructor, isImplGenerICollec, itemType });
                    }
                    else
                    {
                        DEBUG.Assert(args.Length == 1 && args[0].ParameterType == typeof(int));

                        var methodinfo = dynamicCacheType.GetMethod(nameof(CollectionDynamicDelegateCache<int>.GenerateDeserializeWithCapacityCtor));
                        methodinfo.Invoke(null, new object[] { constructor, isImplGenerICollec, itemType });
                    }
                    TypeBuildHelper.CallDeserializeDelegate(deserializeMethod, type, dynamicCacheType.GetField(nameof(CollectionDynamicDelegateCache<int>.Deserialize), BindingFlags.Public | BindingFlags.Static));
                }
            }
            return typeBuilder.CreateTypeInfo();
        }
    }

    internal static class DateTimeCollectionDeserializeBuilder<T> where T : ICollection<DateTime>
    {
        private delegate T Ctor(ref BssomReader reader, int count);
        private static Ctor constructor;

        public static T Deserialize(ref BssomReader reader, ref BssomDeserializeContext context)
        {
            if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array1))
                return default;

            var type = reader.ReadBssomType();
            switch (type)
            {
                case BssomType.TimestampCode:
                    {
                        reader.SkipVariableNumber();
                        int len = reader.ReadVariableNumber();
                        T t = constructor(ref reader, len);
                        for (int i = 0; i < len; i++)
                        {
                            context.CancellationToken.ThrowIfCancellationRequested();
                            t.Add(reader.ReadStandDateTimeWithOutTypeHead());
                        }
                        return t;
                    }
                case BssomType.NativeCode:
                    {
                        reader.EnsureType(NativeBssomType.DateTimeCode);
                        reader.SkipVariableNumber();
                        int len = reader.ReadVariableNumber();
                        T t = constructor(ref reader, len);
                        for (int i = 0; i < len; i++)
                        {
                            context.CancellationToken.ThrowIfCancellationRequested();
                            t.Add(reader.ReadNativeDateTimeWithOutTypeHead());
                        }
                        return t;
                    }
                default:
                    throw BssomSerializationOperationException.UnexpectedCodeRead(type, reader.Position);
            }
        }

        public static void ConstructorInit(ConstructorInfo ctor)
        {
            var paras = ctor.GetParameters();
            ParameterExpression count = Expression.Parameter(typeof(int));
            Expression body;
            if (paras.Length == 0)
                body = Expression.New(ctor);
            else
                body = Expression.New(ctor, count);
            constructor = Expression.Lambda<Ctor>(body, CommonExpressionMeta.Par_Reader, count).Compile();
        }
    }

    internal static class CollectionDynamicDelegateCache<T>
    {
        public static Deserialize<T> Deserialize;

        public static void GenerateDeserializeWithEmptyCtor(ConstructorInfo constructor, bool isImplGenerICollec, Type itemType)
        {
            GenerateDeserializeWithCore(isImplGenerICollec, itemType, (len) => Expression.New(constructor));
        }

        public static void GenerateDeserializeWithCapacityCtor(ConstructorInfo constructor, bool isImplGenerICollec, Type itemType)
        {
            GenerateDeserializeWithCore(isImplGenerICollec, itemType, (len) => Expression.New(constructor, len));
        }

        private static void GenerateDeserializeWithCore(bool isImplGenerICollec, Type itemType, Func<Expression, Expression> ctor)
        {
            /*
               if (reader.TryReadNullWithEnsureArray1BuildInType(BssomType.Int8Code))/TryReadNullWithEnsureArray1NativeType(NativeBssomType.CharCode)/TryReadNullWithEnsureBuildInType(BssomType.Array2)
                    return default;
               context.option.Security.DepthStep(ref reader);
               reader.SkipVariableNumber();
               int len = reader.ReadVariableNumber();
               T t = new T(len);
               Fill<T>(ref t,ref reader,ref context,len);
               context.Depth--;
               return t;  
            */

            bool isArray1Type = Array1FormatterHelper.IsArray1Type(itemType, out bool isNativeType, out byte typeCode, out var typeCodeName);
            var t = typeof(T);
            List<Expression> ary = new List<Expression>(7);
            LabelTarget returnTarget = Expression.Label(t, "returnLable");

            if (isArray1Type)
            {
                if (isNativeType)
                {
                    //if (reader.ryReadNullWithEnsureArray1NativeType(NativeType))
                    //      goto label;
                    ary.Add(Expression.IfThen(CommonExpressionMeta.Call_Reader_TryReadNullWithEnsureArray1NativeType(typeCode), Expression.Return(returnTarget, Expression.Default(t))));
                }
                else
                {
                    //if (reader.Call_Reader_TryReadNullWithEnsureArray1BuildInType(BuildInType))
                    //      goto label;
                    ary.Add(Expression.IfThen(CommonExpressionMeta.Call_Reader_TryReadNullWithEnsureArray1BuildInType(typeCode), Expression.Return(returnTarget, Expression.Default(t))));
                }
            }
            else
            {
                //if (reader.TryReadNullWithEnsureBuildInType(BssomType.Array2))
                //      goto label;
                ary.Add(Expression.IfThen(CommonExpressionMeta.Call_Reader_TryReadNullWithEnsureBuildInType(BssomType.Array2), Expression.Return(returnTarget, Expression.Default(t))));
            }

            //context.option.Security.DepthStep(ref reader);
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Option_Security_DepthStep);

            //reader.SkipVariableNumber();
            ary.Add(CommonExpressionMeta.Call_Reader_SkipVariableNumber);

            //int len = reader.ReadVariableNumber();
            var len = Expression.Variable(typeof(int));
            ary.Add(Expression.Assign(len, CommonExpressionMeta.Call_Reader_ReadVariableNumber));

            //T t = ctor(len);
            ParameterExpression instance = Expression.Variable(t);
            ary.Add(Expression.Assign(instance, ctor(len)));
            MethodInfo method = null;
            if (isImplGenerICollec == false)
            {
                //IColloctionFormatterHelper.Fill_ImplIList<T>(ref t,ref reader,ref context,len)
                method = typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.Fill_ImplIList), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(t);
            }
            else
            {
                if (isArray1Type)
                {
                    //IColloctionFormatterHelper.Fill{TypeCodeName}<T>(ref t,ref reader,ref context,len)
                    method = typeof(Array1FormatterHelper).GetMethod(Array1FormatterHelper.FillPrefix + typeCodeName.ToString().Replace("Code", ""), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(t);
                }
                else
                {
                    //IColloctionFormatterHelper.Fill_ImplICollection<T,TElement>(ref t,ref reader,ref context,len)
                    method = typeof(Array2FormatterHelper).GetMethod(nameof(Array2FormatterHelper.Fill_ImplICollection), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { t, itemType });
                }
            }

            ary.Add(Expression.Call(null, method, instance, CommonExpressionMeta.Par_Reader, CommonExpressionMeta.Par_DeserializeContext, len));
            //context.Depth--;
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Depth_DecrementAssign);
            //return t;
            ary.Add(Expression.Return(returnTarget, instance));
            //label default(T)
            ary.Add(Expression.Label(returnTarget, instance));

            var block = Expression.Block(new ParameterExpression[] { instance, len }, ary);
            Deserialize = Expression.Lambda<Deserialize<T>>(block, CommonExpressionMeta.Par_Reader, CommonExpressionMeta.Par_DeserializeContext).Compile();


        }

        public static void GenerateInjectCtor(ConstructorInfo constructor, Type injectType)
        {
            Deserialize = Expression.Lambda<Deserialize<T>>(CommonExpressionMeta.GenerateInjectCtor(typeof(T), constructor, injectType), CommonExpressionMeta.Par_Reader,  CommonExpressionMeta.Par_DeserializeContext).Compile();
        }
    }
}
