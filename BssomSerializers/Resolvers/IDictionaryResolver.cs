using Bssom.Serializer.Binary;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Resolver;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Linq.Expressions;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Formatters;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.Resolver
{
    /// <summary>
    /// <para>获取和生成具有IDictionary行为的类型的<see cref="IBssomFormatter"/></para>
    /// <para>Get and generate the type with IDictionary behavior <see cref="IBssomFormatter"/></para>
    /// </summary>
    public class IDictionaryResolver : IFormatterResolver
    {
        internal const string ModuleName = "Bssom.Resolvers.IDictionaryResolver";
        internal static readonly DynamicFormatterAssembly DynamicAssembly;

        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly IDictionaryResolver Instance;

        static IDictionaryResolver()
        {
            Instance = new IDictionaryResolver();
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

                if (t == typeof(IDictionary))
                {
                    Formatter = (IBssomFormatter<T>)((IBssomFormatter)IDictionaryFormatter.Instance);
                    return;
                }

                if (TypeIsDictionary(t, out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType))
                {
                    if (typeIsGeneric && genericKeyType != typeof(object) && !BssMapKeyResolverProvider.TryGetBssMapKeyResolver(genericKeyType, out var keyConvert))
                        return;// throw BssomSerializationException.MapKeyTypeError();

                    TypeInfo buildType;
                    if (typeIsGeneric)
                    {
                        if (t.IsInterface)
                            buildType = IDictionaryFormatterTypeBuilder.BuildGenericIDictionaryInterfaceType(DynamicAssembly, t, genericTypeDefinition, genericKeyType, genericValueType);
                        else
                            buildType = IDictionaryFormatterTypeBuilder.BuildGenericIDictionaryImplementationType(DynamicAssembly, constructor, t, genericKeyType, genericValueType);
                    }
                    else
                    {
                        //impl IDictionary class/struct
                        buildType = IDictionaryFormatterTypeBuilder.BuildIDictionaryImplementationType(DynamicAssembly, constructor, t);
                    }

                    Formatter = (IBssomFormatter<T>)Activator.CreateInstance(buildType);
                }
            }
        }


        internal static bool TypeIsDictionary(Type t, out ConstructorInfo constructor, out bool typeIsGeneric, out Type genericTypeDefinition, out Type genericKeyType, out Type genericValueType)
        {
            constructor = null;
            typeIsGeneric = false;
            genericKeyType = null;
            genericValueType = null;
            genericTypeDefinition = null;

            Type genericType = null;
            bool hasIDictionaryGeneric = false;
            bool hasIDictionary = false;

            if (t.IsInterface)
            {
                if (t == typeof(IDictionary))
                {
                    return true;
                }

                if (t.IsGenericType)
                {
                    genericTypeDefinition = t.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(IDictionary<,>) || genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                    {
                        Type[] args = t.GetGenericArguments();
                        typeIsGeneric = true;
                        genericType = t;
                        genericKeyType = args[0];
                        genericValueType = args[1];
                        return true;
                    }
                }
                return false;
            }

            if (t.IsGenericType)
            {
                genericTypeDefinition = t.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Dictionary<,>) ||
                    genericTypeDefinition == typeof(SortedList<,>))
                {
                    constructor = t.GetAppointTypeCtor(typeof(int));
                    typeIsGeneric = true;
                    var args = t.GetGenericArguments();
                    genericKeyType = args[0];
                    genericValueType = args[1];
                    return true;
                }
            }

            IEnumerable<Type> intserfaces = t.GetInterfaces();
            foreach (var item in intserfaces)
            {
                if (item.IsGenericType)
                {
                    genericTypeDefinition = item.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(IDictionary<,>) ||
                        genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                    {
                        genericType = item;
                        var args = item.GetGenericArguments();
                        genericKeyType = args[0];
                        genericValueType = args[1];
                        hasIDictionaryGeneric = true;
                        break;
                    }
                }
                else if (item == typeof(IDictionary))
                    hasIDictionary = true;
            }

            if (hasIDictionaryGeneric)
            {
                typeIsGeneric = true;

                //class <T>:IDictionary<int,int>
                //     ctor( ReadOnlyDic<,> )
                //     ctor( Dictionary<,>  )
                if (TryGetConstructorInfo(t, genericKeyType, genericValueType, true, out constructor))
                {
                    return true;
                }
            }
            else if (hasIDictionary)
            {
                constructor = t.GetDefaultNoArgCtorOrAppointTypeCtor(typeof(IDictionary));
                if (constructor != null)
                    return true;
            }

            return false;
        }

        private static bool TryGetConstructorInfo(Type targetType, Type genericKeyType, Type genericValueType, bool isFindEmptyCtor, out ConstructorInfo constructor)
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


                    if (TypeIsDictionary(ctorArgType, out ConstructorInfo cons, out bool tIsGener, out Type generTypeDefine, out Type generKeyType, out Type generValueType))
                    {
                        if (tIsGener == false ||
                          (generKeyType == genericKeyType && generValueType == genericValueType))
                        {
                            constructor = item;
                            if (!isFindEmptyCtor)
                                return true;
                        }
                    }
                }
            }
            return constructor != null;
        }
    }


}
namespace Bssom.Serializer.Internal
{
    internal static class IDictionaryFormatterTypeBuilder
    {
        //apply to IDictionary<>/IReadOnlyDictionary<>
        public static TypeInfo BuildGenericIDictionaryInterfaceType(DynamicFormatterAssembly assembly, Type type, Type genericTypeDefine, Type genericKeyType, Type genericValueType)
        {
            DEBUG.Assert(genericTypeDefine == typeof(IDictionary<,>) || genericTypeDefine == typeof(IReadOnlyDictionary<,>));

            TypeBuilder typeBuilder = assembly.DefineFormatterType(type);

            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.SerializeGenericDictionary), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { genericKeyType, genericValueType }));

            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            if (genericTypeDefine == typeof(IDictionary<,>))
            {
                TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.GenericDictionaryDeserialize), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { genericKeyType, genericValueType }));
            }
            else
            {
                TypeBuildHelper.CallOneMethodInDeserialize(deserializeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.ReadOnlyGenericDictionaryDeserialize), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { genericKeyType, genericValueType }));
            }

            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);
            TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.SizeGenericDictionary), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericKeyType,genericValueType));


            return typeBuilder.CreateTypeInfo();
        }

        //apply to IDictionary<>/IReadOnlyDictionary<> impl
        public static TypeInfo BuildGenericIDictionaryImplementationType(DynamicFormatterAssembly assembly, ConstructorInfo constructor, Type type, Type genericKeyType, Type genericValueType)
        {
            TypeBuilder typeBuilder = assembly.DefineFormatterType(type);

            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.SerializeGenericDictionary), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(new Type[] { genericKeyType, genericValueType }));

            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            var args = constructor.GetParameters();
            Type dynamicCacheType = typeof(IDictionaryDynamicDelegateCache<>).MakeGenericType(type);
            if (args.Length == 0)
            {
                var methodinfo = dynamicCacheType.GetMethod(nameof(IDictionaryDynamicDelegateCache<int>.GenerateDeserializeWithGenericDictEmptyCtor));
                methodinfo.Invoke(null, new object[] { constructor, genericKeyType, genericValueType });
            }
            else
            {
                DEBUG.Assert(args.Length == 1);
                if (args[0].ParameterType == typeof(int))
                {
                    var methodinfo = dynamicCacheType.GetMethod(nameof(IDictionaryDynamicDelegateCache<int>.GenerateDeserializeWithGenericDictCapacityCtor));
                    methodinfo.Invoke(null, new object[] { constructor, genericKeyType, genericValueType });
                }
                else
                {
                    var methodinfo = dynamicCacheType.GetMethod(nameof(IDictionaryDynamicDelegateCache<int>.GenerateInjectCtor));
                    methodinfo.Invoke(null, new object[] { constructor, args[0].ParameterType });
                }
            }
            TypeBuildHelper.CallDeserializeDelegate(deserializeMethod, type, dynamicCacheType.GetField(nameof(IDictionaryDynamicDelegateCache<int>.Deserialize), BindingFlags.Public | BindingFlags.Static));

            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);
            TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.SizeGenericDictionary), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericKeyType, genericValueType));

            return typeBuilder.CreateTypeInfo();
        }

        //apply to IDictionary impl
        public static TypeInfo BuildIDictionaryImplementationType(DynamicFormatterAssembly assembly, ConstructorInfo constructor, Type type)
        {
            TypeBuilder typeBuilder = assembly.DefineFormatterType(type);

            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            TypeBuildHelper.CallOneMethodInSerialize(serializeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.SerializeIDictionary), BindingFlags.Public | BindingFlags.Static));

            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            var args = constructor.GetParameters();
            Type dynamicCacheType = typeof(IDictionaryDynamicDelegateCache<>).MakeGenericType(type);

            if (args.Length == 1)
            {
                DEBUG.Assert(args[0].ParameterType == typeof(IDictionary));

                //return new T(IDictionaryFormatter.Deserialize)
                var methodinfo = dynamicCacheType.GetMethod(nameof(IDictionaryDynamicDelegateCache<int>.GenerateInjectCtor));
                methodinfo.Invoke(null, new object[] { constructor, args[0].ParameterType });
            }
            else
            {
                var methodinfo = dynamicCacheType.GetMethod(nameof(IDictionaryDynamicDelegateCache<int>.GenerateDeserializeWithIDictionaryEmptyCtor));
                methodinfo.Invoke(null, new object[] { });
            }
            TypeBuildHelper.CallDeserializeDelegate(deserializeMethod, type, dynamicCacheType.GetField(nameof(IDictionaryDynamicDelegateCache<int>.Deserialize), BindingFlags.Public | BindingFlags.Static));

            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);
            TypeBuildHelper.CallOneMethodInSize(sizeMethod, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.SizeIDictionary), BindingFlags.Public | BindingFlags.Static));

            return typeBuilder.CreateTypeInfo();
        }
    }

    internal static class IDictionaryDynamicDelegateCache<T>
    {
        public static Deserialize<T> Deserialize;

        public static void GenerateDeserializeWithGenericDictEmptyCtor(ConstructorInfo constructor, Type keyType, Type valueType)
        {
            GenerateDeserializeWithGenericDictCore(keyType, valueType, (count) => Expression.New(constructor));
        }

        public static void GenerateDeserializeWithGenericDictCapacityCtor(ConstructorInfo constructor, Type keyType, Type valueType)
        {
            GenerateDeserializeWithGenericDictCore(keyType, valueType, (count) => Expression.New(constructor, count));
        }

        private static void GenerateDeserializeWithGenericDictCore(Type keyType, Type valueType, Func<MemberExpression, Expression> ctor)
        {
            /*
               var map = MapFormatterHelper.Deserialize(ref reader,ref context);
               if (map == null)
                   return null;
               context.option.Security.DepthStep(ref reader);
               T t = new T();/new T(map.Count)
               Deserialize(IEnumerable<KeyValuePair<TKey, TValue>> pair,(ICollection<KeyValuePair<TKey, TValue>>)t);
               reader = map.Reader; context = map.Context; 
               reader.Seek(map.EndPos);
               context.Depth--;
               return t;  
            */

            ArrayPack<Expression> ary = new ArrayPack<Expression>(10);
            var t = typeof(T);
            LabelTarget returnTarget = Expression.Label(t, "returnLable");
            var map = Expression.Variable(typeof(IMapDataSource<,>).MakeGenericType(keyType, valueType));
            //map = MapFormatterHelper.Deserialize(ref reader,ref context);
            ary.Add(Expression.Assign(map, CommonExpressionMeta.Call_MapFormatterHelper_Deserialize(keyType, valueType)));
            //if (map == null)
            //      goto label;
            ary.Add(Expression.IfThen(Expression.Equal(map, Expression.Constant(null, map.Type)), Expression.Return(returnTarget, Expression.Default(t))));
            //context.option.Security.DepthStep(ref reader);
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Option_Security_DepthStep);
            //T t = ctor(map.Count);
            ParameterExpression instance = Expression.Variable(t);
            ary.Add(Expression.Assign(instance, ctor(Expression.Property(map, nameof(BssMapObjMarshalReader<int, int>.Count)))));
            //MapFormatterHelper.FillData(map,(ICollection<KeyValuePair<TKey, TValue>>)t)
            ary.Add(Expression.Call(null, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.FillGenericIDictionaryData), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(keyType, valueType), map, Expression.Convert(instance, typeof(ICollection<>).MakeGenericType(typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType)))));

            //reader = map.Reader; context = map.Context; 
            ary.Add(CommonExpressionMeta.Block_MapReaderAndContextAssignLocalReaderAndContext(map));
            //reader.Seek(map.EndPos);
            ary.Add(CommonExpressionMeta.Call_Reader_BufferSeek(Expression.Property(map, nameof(IMapDataSource<int, int>.EndPosition))));
            //context.Depth--;
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Depth_DecrementAssign);
            //return t;
            ary.Add(Expression.Return(returnTarget, instance));
            //label default(T)
            ary.Add(Expression.Label(returnTarget, instance));

            var block = Expression.Block(new ParameterExpression[] { map, instance }, ary.GetArray());
            Deserialize = Expression.Lambda<Deserialize<T>>(block, CommonExpressionMeta.Par_Reader, CommonExpressionMeta.Par_DeserializeContext).Compile();
        }

        public static void GenerateDeserializeWithIDictionaryEmptyCtor()
        {
            /*
              var map = MapFormatterHelper.Deserialize(ref reader,ref context);
              if (map == null)
                  return null;
              context.option.Security.DepthStep(ref reader);
              T t = new T();
              Deserialize(IEnumerable<KeyValuePair<TKey, TValue>> pair,(ICollection<KeyValuePair<TKey, TValue>>)t);
              reader = map.Reader; context = map.Context; 
              reader.Seek(map.EndPos);
              context.Depth--;
              return t;  
           */

            ArrayPack<Expression> ary = new ArrayPack<Expression>(10);
            var t = typeof(T);
            LabelTarget returnTarget = Expression.Label(t, "returnLable");
            var map = Expression.Variable(typeof(IMapDataSource<,>).MakeGenericType(typeof(object), typeof(object)));
            //map = MapFormatterHelper.Deserialize(ref reader,ref context);
            ary.Add(Expression.Assign(map, CommonExpressionMeta.Call_MapFormatterHelper_Deserialize(typeof(object), typeof(object))));
            //if (map == null)
            //      goto label;
            ary.Add(Expression.IfThen(Expression.Equal(map, Expression.Constant(null, map.Type)), Expression.Return(returnTarget, Expression.Default(t))));
            //context.option.Security.DepthStep(ref reader);
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Option_Security_DepthStep);
            //T t = new T();
            ParameterExpression instance = Expression.Variable(t);
            ary.Add(Expression.Assign(instance, Expression.New(t)));
            //MapFormatterHelper.FillData(map,(IDictionary)t)
            ary.Add(Expression.Call(null, typeof(MapFormatterHelper).GetMethod(nameof(MapFormatterHelper.FillIDictionaryData), BindingFlags.Public | BindingFlags.Static), map, Expression.Convert(instance, typeof(IDictionary))));

            //reader = map.Reader; context = map.Context; 
            ary.Add(CommonExpressionMeta.Block_MapReaderAndContextAssignLocalReaderAndContext(map));
            //reader.Seek(map.EndPos);
            ary.Add(CommonExpressionMeta.Call_Reader_BufferSeek(Expression.Property(map, nameof(IMapDataSource<int, int>.EndPosition))));
            //context.Depth--;
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Depth_DecrementAssign);
            //return t;
            ary.Add(Expression.Return(returnTarget, instance));
            //label default(T)
            ary.Add(Expression.Label(returnTarget, instance));

            var block = Expression.Block(new ParameterExpression[] { map, instance }, ary.GetArray());
            Deserialize = Expression.Lambda<Deserialize<T>>(block, CommonExpressionMeta.Par_Reader, CommonExpressionMeta.Par_DeserializeContext).Compile();
        }

        public static void GenerateInjectCtor(ConstructorInfo constructor, Type injectType)
        {
            Deserialize = Expression.Lambda<Deserialize<T>>(CommonExpressionMeta.GenerateInjectCtor(typeof(T), constructor, injectType), CommonExpressionMeta.Par_Reader, CommonExpressionMeta.Par_DeserializeContext).Compile();
        }
    }
}
