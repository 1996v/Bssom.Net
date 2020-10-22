//using System.Runtime.CompilerServices;


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using Bssom.Serializer.Resolver;
using System.Buffers;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;

namespace Bssom.Serializer.Resolver
{
    /// <summary>
    /// <para>对实体(对象)进行动态代码生成BssomMap格式的<see cref="IBssomFormatter"/></para>
    /// <para>The entity(object) is dynamically coded to generate <see cref="IBssomFormatter"/> in BssomMap format</para>
    /// </summary>
    /// <remarks>
    /// <para>只对对象中的公开的可读且可写的元素进行字段编组</para>
    /// <para>Only the readable and writable elements exposed in the object are marshaled</para>
    /// </remarks>
    public sealed class MapCodeGenResolver : IFormatterResolver
    {
        internal const string ModuleName = "Bssom.Serializer.Resolvers.MapCodeGenResolver";
        internal static readonly DynamicFormatterAssembly DynamicAssembly;

        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly MapCodeGenResolver Instance;

        static MapCodeGenResolver()
        {
            Instance = new MapCodeGenResolver();
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
                Formatter = (IBssomFormatter<T>)Activator.CreateInstance(MapCodeGenResolverBuilder.Build(DynamicAssembly, t));
            }
        }

#if NETFRAMEWORK 
        public AssemblyBuilder Save()
        {
            return DynamicAssembly.Save();
        }
#endif
    }
}

namespace Bssom.Serializer.Internal
{
    internal static class MapCodeGenResolverBuilder
    {
        public static TypeInfo Build(DynamicFormatterAssembly assembly, Type type)
        {
            TypeBuilder typeBuilder = assembly.DefineFormatterType(type);
            var serializationInfo = new ObjectSerializationInfo(type, false);

            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);

            var delegateCacheType = typeof(MapDynamicDelegateCache<>).MakeGenericType(type);
            delegateCacheType.GetMethod(nameof(MapDynamicDelegateCache<int>.Factory), BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { assembly, serializationInfo });

            TypeBuildHelper.CallSerializeDelegate(serializeMethod, type, delegateCacheType.GetField(nameof(MapDynamicDelegateCache<int>.Serialize)));
            TypeBuildHelper.CallSizeDelegate(sizeMethod, type, delegateCacheType.GetField(nameof(MapDynamicDelegateCache<int>.Size)));
            BuildDeserializeCore(deserializeMethod, serializationInfo);

            return typeBuilder.CreateTypeInfo();
        }

        private static void BuildDeserializeCore(MethodBuilder method, ObjectSerializationInfo serializationInfo)
        {
            var il = method.GetILGenerator();
            LocalBuilder instance = il.DeclareLocal(serializationInfo.Type); // [loc:0]
            LocalBuilder aprp = il.DeclareLocal(typeof(BssMapHeadPackInfo)); // [loc:1]
            LocalBuilder refb = il.DeclareLocal(typeof(byte).MakeByRefType()); // [loc:2]
            LocalBuilder hesacbf = il.DeclareLocal(typeof(bool)); // [loc:3]
            LocalBuilder depth = il.DeclareLocal(typeof(int)); // [loc:4]
            LocalBuilder type = il.DeclareLocal(typeof(byte)); // [loc:5]

            const int args_reader = 1;
            const int args_deserializeContext = 2;

            //if (reader.TryReadNull())
            //    return default;
            Label falseLabel = il.DefineLabel();
            il.EmitLdarg(args_reader);
            il.Emit(OpCodes.Call, CommonExpressionMeta.Type_TryReadNull);
            il.Emit(OpCodes.Brfalse/*_S*/, falseLabel);
            if (serializationInfo.Type.IsClass)
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.EmitLdloca(instance.LocalIndex);
                il.Emit(OpCodes.Initobj, serializationInfo.Type);
                il.EmitLdloc(instance.LocalIndex);
                il.Emit(OpCodes.Ret);
            }
            il.MarkLabel(falseLabel);

            //context.option.Security.DepthStep(ref reader);
            il.EmitLdarg(args_deserializeContext);
            il.EmitCall(CommonExpressionMeta.Type_DeserializeContext_Option.GetGetMethod());
            il.EmitCall(CommonExpressionMeta.Type_OptionSecurity.GetGetMethod());
            il.EmitLdarg(args_deserializeContext);
            il.Emit(OpCodes.Callvirt, CommonExpressionMeta.Type_OptionSecurity_DepthStep);

            //T instance = new T();
            if (serializationInfo.Type.IsValueType)
            {
                if (serializationInfo.IsDefaultNoArgsCtor)
                {
                    il.EmitLdloca(instance.LocalIndex);
                    il.Emit(OpCodes.Initobj, serializationInfo.Type);
                }
                else
                {
                    il.EmitLdloca(instance.LocalIndex);
                    var parInfos = serializationInfo.BestmatchConstructor.GetParameters();
                    if (serializationInfo.ConstructorParametersIsDefaultValue)
                    {
                        for (int i = 0; i < parInfos.Length; i++)
                        {
                            il.EmitDefault(parInfos[i].ParameterType);
                        }
                    }
                    else
                    {
                        var cpcf = serializationInfo.StoreConstructorParameters();
                        for (int i = 0; i < parInfos.Length; i++)
                        {
                            il.Emit(OpCodes.Ldsfld, cpcf);
                            il.EmitLdc_I4(i);
                            il.Emit(OpCodes.Ldelem_Ref);
                            il.EmitObjectConvertTo(parInfos[i].ParameterType);
                        }
                    }
                    il.Emit(OpCodes.Call, serializationInfo.BestmatchConstructor);
                }
            }
            else
            {
                if (!serializationInfo.IsDefaultNoArgsCtor)
                {
                    var parInfos = serializationInfo.BestmatchConstructor.GetParameters();
                    if (serializationInfo.ConstructorParametersIsDefaultValue)
                    {
                        for (int i = 0; i < parInfos.Length; i++)
                        {
                            il.EmitDefault(parInfos[i].ParameterType);
                        }
                    }
                    else
                    {
                        var cpcf = serializationInfo.StoreConstructorParameters();
                        for (int i = 0; i < parInfos.Length; i++)
                        {
                            il.Emit(OpCodes.Ldsfld, cpcf);
                            il.EmitLdc_I4(i);
                            il.Emit(OpCodes.Ldelem_Ref);
                            il.EmitObjectConvertTo(parInfos[i].ParameterType);
                        }
                    }
                }

                il.Emit(OpCodes.Newobj, serializationInfo.BestmatchConstructor);
                il.EmitStloc(instance.LocalIndex);
            }

            //type = reader.SkipBlankCharacterAndReadBssomType()
            il.EmitLdarg(args_reader);
            il.Emit(OpCodes.Call, CommonExpressionMeta.Type_SkipBlankCharacterAndReadBssomType);
            il.EmitStloc(type.LocalIndex);

            Label endLabel = il.DefineLabel();
            Label map2Label = il.DefineLabel();
            //if(type == BssomType.Map1)
            {
                il.EmitLdloc(type.LocalIndex);
                il.EmitLdc_I4(BssomType.Map1);
                il.Emit(OpCodes.Bne_Un/*_S*/, map2Label);//type != BssomType.Map1 => goto map2Label

                //call MapDynamicDelegateCache<T>.Map1FormatterDeserialize(ref instance, ref reader, ref context);
                il.Emit(OpCodes.Ldsfld, typeof(MapDynamicDelegateCache<>).MakeGenericType(serializationInfo.Type).GetField(nameof(MapDynamicDelegateCache<int>.Map1FormatterDeserialize)));
                il.EmitLdarg(args_deserializeContext);
                il.EmitLdarg(args_reader);
                il.EmitLdloca(instance.LocalIndex);
                il.EmitCall(typeof(Map1FormatterDeserialize<>).MakeGenericType(serializationInfo.Type).GetMethod("Invoke"));

                //goto endLabel;
                il.Emit(OpCodes.Br, endLabel);
            }
            //else
            {
                il.MarkLabel(map2Label);

                Label map2BodyLabel = il.DefineLabel();
                //if(type != BssomType.Map2)
                il.EmitLdloc(type.LocalIndex);
                il.EmitLdc_I4(BssomType.Map2);
                il.Emit(OpCodes.Beq/*_S*/, map2BodyLabel);

                //throw BssomSerializationOperationException.UnexpectedCodeRead();
                il.Emit(OpCodes.Call, typeof(BssomSerializationOperationException).GetMethod(nameof(BssomSerializationOperationException.UnexpectedCode), BindingFlags.NonPublic | BindingFlags.Static));
                il.Emit(OpCodes.Throw);

                il.MarkLabel(map2BodyLabel);
                //var aprp = BssMapHeadPackInfo.Create(ref reader);
                il.EmitLdarg(args_reader);
                il.Emit(OpCodes.Call, CommonExpressionMeta.Type_MapHeadPackInfo_Create);
                il.EmitStloc(aprp.LocalIndex);

                Label readBodyLabel = il.DefineLabel();
                //if(aprp.MapHead.Count>0)
                {
                    il.EmitLdloc(aprp.LocalIndex);
                    il.Emit(OpCodes.Ldfld, CommonExpressionMeta.Type_MapHeadPackInfo_MapHead);
                    il.Emit(OpCodes.Ldfld, CommonExpressionMeta.Type_MapHead_Count);
                    il.EmitLdc_I4(0);
                    il.Emit(OpCodes.Ble, readBodyLabel);

                    //ref byte refb = ref reader.Buffer.TryReadFixedRef(pars.MapHead.MetaLength, out bool haveEnoughSizeAndCanBeFixed);
                    il.EmitLdarg(args_reader);
                    il.EmitCall(CommonExpressionMeta.Type_Reader_Buffer.GetGetMethod());
                    il.EmitLdloc(aprp.LocalIndex);
                    il.Emit(OpCodes.Ldfld, CommonExpressionMeta.Type_MapHeadPackInfo_MapHead);
                    il.Emit(OpCodes.Ldfld, CommonExpressionMeta.Type_MapHead_MetaLength);
                    il.EmitLdloca(hesacbf.LocalIndex);
                    il.EmitCall(CommonExpressionMeta.Type_Buffer_TryReadFixedRef);
                    il.EmitStloc(refb.LocalIndex);

                    Label isFixedFalseLabel = il.DefineLabel();
                    Label readerSeekLabel = il.DefineLabel();

                    var members = serializationInfo.SerializeMemberInfos;
                    var mpf = serializationInfo.StoreMemberPredefinedNames();
                    var mfif = serializationInfo.StoreMemberFormatterInstances();
                    //if(isFixed)
                    {
                        il.EmitLdloc(hesacbf.LocalIndex);
                        il.Emit(OpCodes.Brfalse, isFixedFalseLabel);

                        for (int i = 0; i < members.Length; i++)
                        {
                            var member = members[i];
                            //instance.Member = AutomatePrefixReader.TryGet<#TypeName#>Value(mpf[i], BssomType.String, ref aprp, ref reader, ref context, ref refb, out haveEnoughSizeAndCanBeFixed);
                            if (serializationInfo.Type.IsValueType)
                                il.EmitLdloca(instance.LocalIndex);
                            else
                                il.EmitLdloc(instance.LocalIndex);
                            il.Emit(OpCodes.Ldsfld, mpf);
                            il.EmitLdc_I4(i);
                            il.Emit(OpCodes.Ldelem_Ref);
                            il.EmitLdc_I4((byte)BssomType.StringCode);
                            il.EmitLdc_I4(0);//false
                            il.EmitLdloca(aprp.LocalIndex);
                            il.EmitLdarg(args_reader);
                            il.EmitLdarg(args_deserializeContext);
                            il.EmitLdloc(refb.LocalIndex);
                            il.EmitLdloca(hesacbf.LocalIndex);
                            if (member.FormatterAttribute != null || !MapCodeGenInlineTypes.TryGetInlineTryGetValueMethod(member.Type, out MethodInfo methodInfo))
                            {
                                //instance.Member = AutomatePrefixReader.TryGetValue<<#TypeName#>>(mpf[i], BssomType.String, false, ref aprp, ref reader,ref context, ref refb, out haveEnoughSizeAndCanBeFixed, null/formatter);
                                methodInfo = BssMapObjMarshalReader.TryGetValueUlongsKeyMethodInfo.MakeGenericMethod(member.Type);

                                if (member.FormatterAttribute == null)
                                    il.Emit(OpCodes.Ldnull);
                                else
                                {
                                    il.Emit(OpCodes.Ldsfld, mfif);
                                    il.EmitLdc_I4(member.GetFormatterAttributeIndex());
                                    il.Emit(OpCodes.Ldelem_Ref);
                                    il.EmitObjectConvertTo(typeof(IBssomFormatter<>).MakeGenericType(member.Type));
                                }
                            }
                            il.EmitCall(methodInfo);
                            il.EmitSetPropertyOrField(member.Member);
                        }

                        //reader.Buffer.UnFixed();
                        il.EmitLdarg(args_reader);
                        il.EmitCall(CommonExpressionMeta.Type_Reader_Buffer.GetGetMethod());
                        il.EmitCall(CommonExpressionMeta.Type_Buffer_UnFixed);

                        //goto endLabel;
                        il.Emit(OpCodes.Br, readerSeekLabel);
                    }
                    //else
                    {
                        il.MarkLabel(isFixedFalseLabel);

                        for (int i = 0; i < members.Length; i++)
                        {
                            var member = members[i];
                            //instance.Member = AutomatePrefixReader.TryGet<#TypeName#>ValueSlow(mpf[i], BssomType.String, false, ref aprp, ref reader, ref context, out haveEnoughSizeAndCanBeFixed);
                            if (serializationInfo.Type.IsValueType)
                                il.EmitLdloca(instance.LocalIndex);
                            else
                                il.EmitLdloc(instance.LocalIndex);
                            il.Emit(OpCodes.Ldsfld, mpf);
                            il.EmitLdc_I4(i);
                            il.Emit(OpCodes.Ldelem_Ref);
                            il.EmitLdc_I4((byte)BssomType.StringCode);
                            il.EmitLdc_I4(0);//false
                            il.EmitLdloca(aprp.LocalIndex);
                            il.EmitLdarg(args_reader);
                            il.EmitLdarg(args_deserializeContext);
                            il.EmitLdloca(hesacbf.LocalIndex);

                            if (member.FormatterAttribute != null || !MapCodeGenInlineTypes.TryGetInlineTryGetValueSlowMethod(member.Type, out MethodInfo methodInfo))
                            {
                                //instance.Member = AutomatePrefixReader.TryGetValueSlow<#TypeName#>(mpf[i], BssomType.String, false, ref aprp, ref reader,ref context, out haveEnoughSizeAndCanBeFixed, null/formatter);
                                methodInfo = BssMapObjMarshalReader.TryGetValueSlowUlongsKeyMethodInfo.MakeGenericMethod(member.Type);

                                if (member.FormatterAttribute == null)
                                    il.Emit(OpCodes.Ldnull);
                                else
                                {
                                    il.Emit(OpCodes.Ldsfld, mfif);
                                    il.EmitLdc_I4(member.GetFormatterAttributeIndex());
                                    il.Emit(OpCodes.Ldelem_Ref);
                                    il.EmitObjectConvertTo(typeof(IBssomFormatter<>).MakeGenericType(member.Type));
                                }
                            }
                            il.EmitCall(methodInfo);
                            il.EmitSetPropertyOrField(member.Member);
                        }
                    }

                    il.MarkLabel(readerSeekLabel);
                    il.EmitLdarg(args_reader);
                    il.Emit(OpCodes.Call, CommonExpressionMeta.Type_Reader_Buffer.GetGetMethod());
                    il.EmitLdloca(aprp.LocalIndex);
                    il.Emit(OpCodes.Call, CommonExpressionMeta.Type_MapHeadPackInfo_DataEndPosition.GetGetMethod());
                    il.EmitLdc_I4((int)BssomSeekOrgin.Begin);
                    il.EmitCall(CommonExpressionMeta.Type_Buffer_Seek);
                }
                il.MarkLabel(readBodyLabel);
            }

            il.MarkLabel(endLabel);

            //context.Depth--;
            il.EmitLdarg(args_deserializeContext);
            il.Emit(OpCodes.Dup);
            il.EmitGetPropertyOrField(CommonExpressionMeta.Type_DeserializeContext_Depth);
            il.EmitStloc(depth.LocalIndex);
            il.EmitLdloc(depth.LocalIndex);
            il.EmitLdc_I4(1);
            il.Emit(OpCodes.Sub);
            il.EmitSetPropertyOrField(CommonExpressionMeta.Type_DeserializeContext_Depth);

            //return instance;
            il.EmitLdloc(instance.LocalIndex);
            il.Emit(OpCodes.Ret);
        }

    }
    internal static class MapDynamicDelegateCache<T>
    {
        public readonly static FieldInfo _MapBuffer = typeof(MapDynamicDelegateCache<T>).GetField(nameof(MapBuffer));

        public static byte[] MapBuffer;
        public static Serialize<T> Serialize;
        public static Size<T> Size;
        public static Map1FormatterDeserialize<T> Map1FormatterDeserialize;

        internal static void Factory(DynamicFormatterAssembly assembly, ObjectSerializationInfo objectSerializationInfo)
        {
            ParameterExpression instance = Expression.Parameter(objectSerializationInfo.Type, "value");
            Expression mapBuffer = Expression.Field(null, _MapBuffer);

            Expression<Serialize<T>> serializeExpression = MapDynamicExpressionBuild.BuildSerializeLambda<T>(objectSerializationInfo, instance, mapBuffer, out MapBuffer);
            Expression<Size<T>> sizeExpression = MapDynamicExpressionBuild.BuildSizeLambda<T>(objectSerializationInfo, instance, mapBuffer);
            Expression<Map1FormatterDeserialize<T>> map1FormatterDeserializeExpression = MapDynamicExpressionBuild.BuildMap1FormatterDeserializeLambda<T>(objectSerializationInfo);

            Serialize = serializeExpression.Compile();
            Size = sizeExpression.Compile();
            Map1FormatterDeserialize = map1FormatterDeserializeExpression.Compile();

#if NETFRAMEWORK 
            TypeBuilder typeBuilder = assembly.DefineFormatterDelegateType(objectSerializationInfo.Type);
            MethodBuilder serializeDelegate = TypeBuildHelper.DefineSerializeDelegate(typeBuilder, typeof(T));
            serializeExpression.CompileToMethod(serializeDelegate);
            MethodBuilder sizeDelegate = TypeBuildHelper.DefineSizeDelegate(typeBuilder, typeof(T));
            sizeExpression.CompileToMethod(sizeDelegate);
            MethodBuilder map1FormatterDeserializeDelegate = TypeBuildHelper.DefineMap1FormatterDeserializeDelegate(typeBuilder, typeof(T));
            map1FormatterDeserializeExpression.CompileToMethod(map1FormatterDeserializeDelegate);
            typeBuilder.CreateTypeInfo();
#endif
        }
    }
    internal static class MayCodeGenDeserializeCache<T>
    {
        public static object[] ConstructorStaticParameters;
        public static ulong[][] PredefinedNames;
        public static IBssomFormatter[] MemberFormatterInstances;
    }
    internal class ObjectSerializationInfo
    {
        private FieldInfo _memberFormatterInstances;

        public Type Type { get; set; }

        public SerializeMemberInfo[] SerializeMemberInfos { get; set; }

        public ConstructorInfo BestmatchConstructor { get; set; }

        public object[] ConstructorParameters { get; set; }

        public bool IsValueType => Type.IsValueType;

        public bool IsInterface => Type.IsInterface;

        public bool IsDefaultNoArgsCtor => BestmatchConstructor == null;

        public bool ConstructorParametersIsDefaultValue => BestmatchConstructor != null && ConstructorParameters == null;

        public ObjectSerializationInfo(Type type, bool allowPrivate)
        {
            Type = type;
            SerializeMemberInfos = GetAllPublicInstanceReadableAndWritableElements(type).ToArray();
            if (type.IsValueType)
            {
                BestmatchConstructor = GetValueTypeCtor(type, out var paras);
                ConstructorParameters = paras;
            }
            else
            {
                if (type.IsInterface)
                {
                    var impType = InterfaceImplementation.CreateType(type);
                    BestmatchConstructor = impType.GetConstructors()[0];
                }
                else
                {
                    BestmatchConstructor = GetClassCtor(type, out var paras);
                    ConstructorParameters = paras;
                }
            }
        }

        public FieldInfo StoreConstructorParameters()
        {
            if (ConstructorParametersIsDefaultValue == false)
            {
                var cspcfield = typeof(MayCodeGenDeserializeCache<>).MakeGenericType(Type).GetField(nameof(MayCodeGenDeserializeCache<int>.ConstructorStaticParameters));
                cspcfield.SetValue(null, ConstructorParameters);
                return cspcfield;
            }
            return null;
        }

        public FieldInfo StoreMemberPredefinedNames()
        {
            if (SerializeMemberInfos.Length > 0)
            {
                var pnfield = typeof(MayCodeGenDeserializeCache<>).MakeGenericType(Type).GetField(nameof(MayCodeGenDeserializeCache<int>.PredefinedNames));
                pnfield.SetValue(null, GetMemberPredefinedNames());
                return pnfield;
            }
            return null;
        }

        public FieldInfo StoreMemberFormatterInstances()
        {
            if (_memberFormatterInstances != null)
                return _memberFormatterInstances;

            int count = SerializeMemberInfos.Count(e => e.FormatterAttribute != null);
            if (count > 0)
            {
                ArrayPack<IBssomFormatter> array = new ArrayPack<IBssomFormatter>(count);
                foreach (var item in SerializeMemberInfos.Where(e => e.FormatterAttribute != null))
                {
                    var formatter = (IBssomFormatter)Activator.CreateInstance(item.FormatterAttribute.FormatterType, item.FormatterAttribute.Arguments);

                    item.SetFormatterAttributeIndex(array.NextPos);
                    array.Add(formatter);
                }

                _memberFormatterInstances = typeof(MayCodeGenDeserializeCache<>).MakeGenericType(Type).GetField(nameof(MayCodeGenDeserializeCache<int>.MemberFormatterInstances));

                _memberFormatterInstances.SetValue(null, array.GetArray());
                return _memberFormatterInstances;
            }
            return null;
        }

        public BssRow<SerializeMemberInfo>[] GetLittle64MemberInfoRows()
        {
            BssRow<SerializeMemberInfo>[] rows = new BssRow<SerializeMemberInfo>[SerializeMemberInfos.Length];
            for (int i = 0; i < SerializeMemberInfos.Length; i++)
            {
                rows[i] = new BssRow<SerializeMemberInfo>(BssMapKeyResolverProvider.StringBssMapKeyResolver.GetMap2KeySegment(SerializeMemberInfos[i].Name), SerializeMemberInfos[i], BssomType.StringCode, false);
            }
            return rows;
        }

        public BssRow<SerializeMemberInfo>[] GetRaw64MemberInfoRows()
        {
            BssRow<SerializeMemberInfo>[] rows = new BssRow<SerializeMemberInfo>[SerializeMemberInfos.Length];
            for (int i = 0; i < SerializeMemberInfos.Length; i++)
            {
                rows[i] = new BssRow<SerializeMemberInfo>(BssMapKeyResolverProvider.StringBssMapKeyResolver.GetMap1KeySegment(SerializeMemberInfos[i].Name), SerializeMemberInfos[i], BssomType.StringCode, false);
            }
            return rows;
        }

        private ulong[][] GetMemberPredefinedNames()
        {
            ulong[][] names = new ulong[SerializeMemberInfos.Length][];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = BssMapStringKeyResolver.Insance.GetMap2KeySegment(SerializeMemberInfos[i].Name).Ts.ToArray();
            }
            return names;
        }

        private static List<SerializeMemberInfo> GetAllPublicInstanceReadableAndWritableElements(Type type)
        {
            List<SerializeMemberInfo> list = new List<SerializeMemberInfo>();
            List<SerializeMemberInfo> includes = new List<SerializeMemberInfo>();
            Type ignoreAttribute = typeof(IgnoreKeyAttribute);
            List<PropertyInfo> pors = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            if (type.IsInterface)
            {
                Type[] interfaces = type.GetInterfaces();
                if (interfaces.Length > 0)
                {
                    foreach (var item in interfaces)
                    {
                        pors.AddRange(item.GetProperties());
                    }
                }
            }

            void AddItem(MemberInfo item, Type itemType)
            {
                //alias
                AliasAttribute alias = item.GetCustomAttribute<AliasAttribute>(false);
                var name = alias != null ? alias.Name : item.Name;

                //formatter
                BssomFormatterAttribute formatter = item.GetCustomAttribute<BssomFormatterAttribute>();

                if (formatter != null)
                {
                    if (formatter.FormatterType.IsGenericType && !formatter.FormatterType.IsConstructedGenericType)
                    {
                        formatter.FormatterType = formatter.FormatterType.MakeGenericType(itemType.GetGenericArguments());
                    }

                    if (!formatter.FormatterType.GetInterfaces().Contains(typeof(IBssomFormatter<>).MakeGenericType(itemType)))
                        throw BssomSerializationTypeFormatterException.AttributeFormatterTypeMismatch(formatter.FormatterType, itemType);
                }

                //OnlyIncludeAttribute
                OnlyIncludeAttribute include = item.GetCustomAttribute<OnlyIncludeAttribute>(false);
                if (include != null)
                    includes.Add(new SerializeMemberInfo(name, itemType, item, formatter));
                else
                    list.Add(new SerializeMemberInfo(name, itemType, item, formatter));
            }

            foreach (var item in pors)
            {
                if (item.IsDefined(ignoreAttribute, false))
                    continue;
                if (item.GetIndexParameters().Length > 0)
                    continue;
                if (!item.CanRead)
                    continue;
                if (!item.CanWrite)
                    continue;

                AddItem(item, item.PropertyType);
            }
            FieldInfo[] fils = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in fils)
            {
                if (item.IsDefined(ignoreAttribute, false))
                    continue;

                AddItem(item, item.FieldType);
            }

            if (includes.Count > 0)
                return includes;
            return list;
        }

        private static ConstructorInfo GetValueTypeCtor(Type type, out object[] paras)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var constructor in constructors)
            {
                var parInfos = constructor.GetParameters();
                SerializationConstructorAttribute sca = constructor.GetCustomAttribute<SerializationConstructorAttribute>();
                if (sca != null)
                {
                    if (sca._paras != null && !VertyConstructorParams(parInfos, sca._paras))
                        throw BssomSerializationTypeFormatterException.CtorParasMismatch(type);
                    paras = sca._paras;
                    return constructor;
                }
            }
            paras = null;
            return null;
        }

        private static ConstructorInfo GetClassCtor(Type type, out object[] paras)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo publicNoArgsCtor = null, publicArgsCtor = null;
            foreach (var constructor in constructors)
            {
                var parInfos = constructor.GetParameters();
                SerializationConstructorAttribute sca = constructor.GetCustomAttribute<SerializationConstructorAttribute>();
                if (sca != null)
                {
                    if (sca._paras != null && !VertyConstructorParams(parInfos, sca._paras))
                        throw BssomSerializationTypeFormatterException.CtorParasMismatch(type);
                    paras = sca._paras;
                    return constructor;
                }

                if (parInfos.Length == 0)
                {
                    publicNoArgsCtor = constructor;
                }
                else if (publicArgsCtor == null || parInfos.Length < publicArgsCtor.GetParameters().Length)
                {
                    publicArgsCtor = constructor;
                    //publicArgsCtorParas = parInfos;
                }
            }

            if (publicNoArgsCtor != null)
            {
                paras = null;
                return publicNoArgsCtor;
            }

            if (publicArgsCtor != null)
            {
                paras = null;
                return publicArgsCtor;
            }

            throw BssomSerializationTypeFormatterException.NotFoundSuitableCtor(type);
        }

        private static bool VertyConstructorParams(ParameterInfo[] parInfos, object[] paras)
        {
            if (parInfos.Length != paras.Length)
                return false;

            for (int i = 0; i < parInfos.Length; i++)
            {
                if (paras[i] == null)
                {
                    if (parInfos[i].ParameterType.IsValueType)
                        return false;
                }
                else if (parInfos[i].ParameterType != paras[i].GetType())
                    return false;
            }

            return true;
        }
    }
    internal class SerializeMemberInfo
    {
        private int _formatterAttributeSlotIndex;

        public string Name;
        public Type Type;
        public MemberInfo Member;
        public BssomFormatterAttribute FormatterAttribute;

        public SerializeMemberInfo(string name, Type type, MemberInfo mem, BssomFormatterAttribute formatterAttribute)
        {
            Name = name;
            Type = type;
            Member = mem;
            FormatterAttribute = formatterAttribute;
        }

        public int GetFormatterAttributeIndex()
        {
            return _formatterAttributeSlotIndex;
        }

        public void SetFormatterAttributeIndex(int index)
        {
            _formatterAttributeSlotIndex = index;
        }
    }
    internal class MapCodeGenInlineTypes
    {
        struct InlineTypeEntry
        {
            public string TypeName;
            public int Size;
            public MethodInfo TryGetValueMethodInfo;
            public MethodInfo TryGetValueSlowMethodInfo;

            public InlineTypeEntry(string typeName, int size, MethodInfo tryGetValueMethodInfo, MethodInfo tryGetValueSlowMethodInfo)
            {
                TypeName = typeName;
                Size = size;
                TryGetValueMethodInfo = tryGetValueMethodInfo;
                TryGetValueSlowMethodInfo = tryGetValueSlowMethodInfo;
            }
        }

        private static Dictionary<Type, InlineTypeEntry> InlineTypes = new Dictionary<Type, InlineTypeEntry>()
        {
            { typeof(Byte),new InlineTypeEntry("UInt8",BssomBinaryPrimitives.UInt8Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt8Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt8ValueSlow))) },
            { typeof(SByte),new InlineTypeEntry("Int8",BssomBinaryPrimitives.Int8Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt8Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt8ValueSlow))) },
            { typeof(Char),new InlineTypeEntry("Char",BssomBinaryPrimitives.CharSize + BssomBinaryPrimitives.NativeTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetCharValue)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetCharValueSlow))) },
            { typeof(Int16),new InlineTypeEntry("Int16",BssomBinaryPrimitives.Int16Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt16Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt16ValueSlow))) },
            { typeof(UInt16),new InlineTypeEntry("UInt16",BssomBinaryPrimitives.UInt16Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt16Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt16ValueSlow))) },
            { typeof(Int32),new InlineTypeEntry("Int32",BssomBinaryPrimitives.Int32Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt32Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt32ValueSlow))) },
            { typeof(UInt32),new InlineTypeEntry("UInt32",BssomBinaryPrimitives.UInt32Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt32Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt32ValueSlow))) },
            { typeof(Int64),new InlineTypeEntry("Int64",BssomBinaryPrimitives.Int64Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt64Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetInt64ValueSlow))) },
            { typeof(UInt64),new InlineTypeEntry("UInt64",BssomBinaryPrimitives.UInt64Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt64Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetUInt64ValueSlow))) },
            { typeof(Single),new InlineTypeEntry("Float32",BssomBinaryPrimitives.Float32Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetFloat32Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetFloat32ValueSlow))) },
            { typeof(Double),new InlineTypeEntry("Float64",BssomBinaryPrimitives.Float64Size + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetFloat64Value)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetFloat64ValueSlow))) },
            { typeof(Decimal),new InlineTypeEntry("Decimal",BssomBinaryPrimitives.DecimalSize + BssomBinaryPrimitives.NativeTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetDecimalValue)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetDecimalValueSlow))) },
            { typeof(Boolean),new InlineTypeEntry("Boolean",BssomBinaryPrimitives.BooleanSize + BssomBinaryPrimitives.BuildInTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetBooleanValue)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetBooleanValueSlow))) },
            { typeof(Guid),new InlineTypeEntry("Guid",BssomBinaryPrimitives.GuidSize + BssomBinaryPrimitives.NativeTypeCodeSize,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetGuidValue)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetGuidValueSlow))) },
            { typeof(DateTime),new InlineTypeEntry("DateTime",-1,typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetDateTimeValue)),typeof(BssMapObjMarshalReader).GetMethod(nameof(BssMapObjMarshalReader.TryGetDateTimeValueSlow))) },
        };

        public static bool TryGetInlineTryGetValueMethod(Type type, out MethodInfo tryGetValueMethod)
        {
            if (InlineTypes.TryGetValue(type, out var entry))
            {
                tryGetValueMethod = entry.TryGetValueMethodInfo;
                return true;
            }
            tryGetValueMethod = default;
            return false;
        }

        public static bool TryGetInlineTryGetValueSlowMethod(Type type, out MethodInfo tryGetValueSlowMethod)
        {
            if (InlineTypes.TryGetValue(type, out var entry))
            {
                tryGetValueSlowMethod = entry.TryGetValueSlowMethodInfo;
                return true;
            }
            tryGetValueSlowMethod = default;
            return false;
        }

        public static bool TryGetSerializeWriteExpression(Expression element, Type type, out Expression writeExpression)
        {
            if (InlineTypes.TryGetValue(type, out var entry))
            {
                if (type == typeof(DateTime))
                    writeExpression = CommonExpressionMeta.Call_WriteDateTime(element);
                else
                    writeExpression = CommonExpressionMeta.Call_Write(element);
                return true;
            }
            writeExpression = default;
            return false;
        }

        public static bool TryGetDeserializeReadExpression(Type type, out Expression writeExpression)
        {
            if (InlineTypes.TryGetValue(type, out var entry))
            {
                writeExpression = CommonExpressionMeta.Call_Read(entry.TypeName);
                return true;
            }
            writeExpression = default;
            return false;
        }

        public static bool TryGetSizeExpression(Expression element, Type type, out Expression sizeExpression)
        {
            if (InlineTypes.TryGetValue(type, out var entry))
            {
                if (type == typeof(DateTime))
                    sizeExpression = CommonExpressionMeta.Call_DateTimeSize();
                else
                    sizeExpression = Expression.Constant(entry.Size);
                return true;
            }
            sizeExpression = default;
            return false;
        }
    }

    internal static class MapDynamicExpressionBuild
    {
        #region Serialize

        public static Expression<Serialize<T>> BuildSerializeLambda<T>(ObjectSerializationInfo serializationInfo, ParameterExpression instance, Expression mapBufferField, out byte[] mapBuffer)
        {
            return Expression.Lambda<Serialize<T>>(BuildSerializeCore(typeof(T), serializationInfo, instance, mapBufferField, out mapBuffer), CommonExpressionMeta.Par_Writer, CommonExpressionMeta.Par_SerializeContext, instance);
        }

        private static Expression BuildSerializeCore(Type type, ObjectSerializationInfo serializationInfo, ParameterExpression instance, Expression mapBufferField, out byte[] mapBuffer)
        {
            var bw = ExpandableBufferWriter.CreateTemporary();
            var cw = new BssomWriter(bw);
            var valueMapOffsets = new BssMapObjMarshal<SerializeMemberInfo>(serializationInfo.GetLittle64MemberInfoRows()).WriteHeader(ref cw);
            mapBuffer = bw.GetBufferedArray();

            List<Expression> ary = new List<Expression>();
            LabelTarget returnTarget = Expression.Label(typeof(void), "returnLable");

            if (!type.IsValueType)
            {
                //if (value==null)
                //     writer.WriteNull(); goto label;
                ary.Add(CommonExpressionMeta.Block_IfNullWriteNullWithReturn(instance, type, returnTarget));
            }

            //writer.WriteBuildInType(BssomType.Map2)
            ary.Add(CommonExpressionMeta.Call_WriteBuildInType(Expression.Constant((byte)BssomType.Map2, typeof(byte))));

            //writer.Write(mapBuf);
            ary.Add(CommonExpressionMeta.Call_WriteRaw(mapBufferField));

            ParameterExpression[] variables = null;

            if (valueMapOffsets.Count > 0)
            {
                variables = new ParameterExpression[valueMapOffsets.Count + 2];
                //ApmWriteBackEntry entry1,entry2,entry3...
                for (int i = 0; i < variables.Length - 2; i++)
                {
                    variables[i] = Expression.Variable(typeof(BssMapWriteBackEntry), $"entry{i}");
                }
                //long startPos,end; 
                ParameterExpression startPos = Expression.Variable(typeof(long), "startPos");
                variables[variables.Length - 2] = startPos;
                ParameterExpression end = Expression.Variable(typeof(long), "end");
                variables[variables.Length - 1] = end;

                //startPos = writer.Pos - mapBuffer.Length;
                ary.Add(Expression.Assign(startPos, Expression.Subtract(CommonExpressionMeta.Field_WriterPos, Expression.Constant((long)mapBuffer.Length))));

                var memFormatters = serializationInfo.StoreMemberFormatterInstances();
                int n = 0;
                while (valueMapOffsets.Count > 0)
                {
                    var mem = valueMapOffsets.Dequeue();

                    //entry{n}.ValueOffset = (uint)(writer.Pos - startPos);
                    ary.Add(Expression.Assign(Expression.Field(variables[n], BssMapWriteBackEntry._ValueOffset), Expression.Convert(Expression.Subtract(CommonExpressionMeta.Field_WriterPos, startPos), typeof(uint))));

                    //entry{n}.MapOffset = {mem.Key};  
                    ary.Add(Expression.Assign(Expression.Field(variables[n], BssMapWriteBackEntry._MapOffset), Expression.Constant(mem.Key, typeof(int))));

                    MemberExpression ele;
                    if (mem.Value.Member is FieldInfo f)
                        ele = Expression.Field(instance, f);
                    else
                        ele = Expression.Property(instance, (PropertyInfo)mem.Value.Member);

                    if (mem.Value.FormatterAttribute != null)
                    {
                        //formatter.Serialize(ref writer,instance.{mem}, option) 
                        ary.Add(CommonExpressionMeta.Call_FormatterSerialize(Expression.Convert(
                            Expression.ArrayIndex(Expression.Field(null, memFormatters), Expression.Constant(mem.Value.GetFormatterAttributeIndex())),
                            typeof(IBssomFormatter<>).MakeGenericType(mem.Value.Type)), ele));
                    }
                    else
                    {
                        if (MapCodeGenInlineTypes.TryGetSerializeWriteExpression(ele, mem.Value.Type, out Expression inlineWriteExpr))
                        {
                            ary.Add(inlineWriteExpr);
                        }
                        else
                        {
                            //option.Formatter.GetFormatterWithVerify().Serialize(ref writer,instance.{mem}, option)
                            ary.Add(CommonExpressionMeta.Call_SerializeContextOptionResolver_GetFormatterWithVerify_Serialize(ele));
                        }
                    }

                    n++;
                }

                //end = writer.Pos;
                ary.Add(Expression.Assign(end, CommonExpressionMeta.Field_WriterPos));

                // writer.WriteFixUInt32WithRefPos((uint)(end - startPos - BssMapObjMarshal.DefaultMapLengthFieldSize),startPos);
                ary.Add(CommonExpressionMeta.Call_WriteFixUInt32WithRefPos(Expression.Convert(Expression.Subtract(Expression.Subtract(end, startPos), Expression.Constant((long)BssMapObjMarshal.DefaultMapLengthFieldSize)), typeof(uint)), startPos));

                for (int i = 0; i < variables.Length - 2; i++)
                {
                    // writer.WriteFixUInt32WithRefPos(entry{i}.ValueOffset, entry{i}.MapOffset + startPos);
                    ary.Add(CommonExpressionMeta.Call_WriteFixUInt32WithRefPos(Expression.Field(variables[i], BssMapWriteBackEntry._ValueOffset), Expression.Add(Expression.Convert(Expression.Field(variables[i], BssMapWriteBackEntry._MapOffset), typeof(long)), startPos)));
                }

                //writer.Seek(end)
                ary.Add(CommonExpressionMeta.Call_Writer_Seek(end));
            }

            ary.Add(Expression.Label(returnTarget));

            return Expression.Block(variables, ary);
        }

        #endregion

        #region Map1FormatterDeserialize

        public static Expression<Map1FormatterDeserialize<T>> BuildMap1FormatterDeserializeLambda<T>(ObjectSerializationInfo serializationInfo)
        {
            var instance = Expression.Parameter(typeof(T).MakeByRefType(), "instance");
            return Expression.Lambda<Map1FormatterDeserialize<T>>(BuildMap1FormatterDeserializeCore(serializationInfo, instance), CommonExpressionMeta.Par_DeserializeContext, CommonExpressionMeta.Par_Reader, instance);
        }

        private static Expression BuildMap1FormatterDeserializeCore(ObjectSerializationInfo serializationInfo, ParameterExpression instance)
        {
            List<Expression> ary = new List<Expression>();

            //ulong key;
            var key = Expression.Variable(typeof(ulong), "key");
            //int num;
            var num = Expression.Variable(typeof(int), "num");
            //int len;
            var len = Expression.Variable(typeof(int), "len");
            //int for-i;
            var forVariable = Expression.Variable(typeof(int), "i");

            //reader.SkipVariableNumber()
            ary.Add(CommonExpressionMeta.Call_Reader_SkipVariableNumber);
            //num = reader.ReadVariableNumber()
            ary.Add(Expression.Assign(num, CommonExpressionMeta.Call_Reader_ReadVariableNumber));

            //while (true)
            //     if(i<num){
            //          len = reader.GetMapStringKeyLength();
            //          ulong = reader.ReadRaw64(ref len)
            //          if(len == 0){ logic;  }
            //          switch(ulong){ logic; }
            //       Incre:
            //          i++;
            //          continue;
            //       seekAndSkip:
            //          reader.SeekAndSkipObject(len);
            //          goto Incre;
            //    }
            var seekLabel = Expression.Label("seekAndSkip");
            var increLabel = Expression.Label("increment");
            var breakLabel = Expression.Label("LoopBreak");
            var continueLabel = Expression.Label("continue");
            var ap = new BssMapObjMarshal<SerializeMemberInfo>(serializationInfo.GetRaw64MemberInfoRows());
            var body = Expression.IfThenElse(
                Expression.LessThan(forVariable, num),
                Expression.Block(
                    Expression.Assign(len, CommonExpressionMeta.Call_ReaderGetMapStringKeyLength),
                    BuildMap1FormatterDeserializeCore_ReadBranch(instance, len, key, seekLabel, serializationInfo, ap.Entries, 0, ap.Length),
                    Expression.Label(increLabel),
                    Expression.PostIncrementAssign(forVariable),
                    Expression.Continue(continueLabel),
                    Expression.Label(seekLabel), CommonExpressionMeta.Call_Reader_SeekAndSkipObject(len), Expression.Goto(increLabel)
                ),
                Expression.Break(breakLabel)
            );

            ary.Add(Expression.Loop(body, breakLabel, continueLabel));
            return Expression.Block(new ParameterExpression[] { key, num, len, forVariable }, ary);
        }

        private static Expression BuildMap1FormatterDeserializeCore_ReadBranch(ParameterExpression instance, ParameterExpression len, ParameterExpression key, LabelTarget skip, ObjectSerializationInfo serializationInfo, BssMapObjMarshal<SerializeMemberInfo>.Entry[] entries, int start, int entryLength)
        {
            //key = reader.ReadRaw64(ref len);
            var readRaw = Expression.Assign(key, CommonExpressionMeta.Call_Reader_ReadRaw64Ref(len));
            if (entryLength == 0)
            {
                return Expression.Block(readRaw, Expression.Goto(skip));
            }

            Expression combin;
            var zeroGroup = entries.Skip(start).Take(entryLength).GroupBy(e => e.IsKey);
            var childrenGroup = entries.Skip(start).Take(entryLength).GroupBy(e => e.Chidlerns == null);

            if (zeroGroup.Where(e => e.Key).Any())
            {
                var up = zeroGroup.Where(e => e.Key).First().ToArray();
                bool isMoveDown = up.Where(e => e.Chidlerns != null).Count() > 0;
                /*
                    (false) -  (true)  -  (true)
                            -  (true)  -  (true)
                            -  (false) -  (true)

                    if(len == 0)
                        switch 1/2 
                    else
                        read 1,  switch4
                        read 2,  switch5
                        read 3,  switch6
                             
                    (false) - 1(true)  -  4(true)
                            - 2(true)  -  5(true)
                            - 3(false) -  6(true)
                 */

                for (int i = 0; i < up.Length; i++)
                {
                    up[i] = up[i].Clone().WithNotChildren();
                }

                var ifZero = BuildMap1FormatterDeserializeCore_SwtichCase(instance, len, key, skip, serializationInfo, up, 0, up.Length);

                if (up.Length == entryLength && !isMoveDown)
                {
                    //if(len == 0)
                    //   swtich...
                    //else
                    //   Skip
                    combin = Expression.IfThenElse(Expression.Equal(len, Expression.Constant(0)), ifZero, Expression.Goto(skip));
                }
                else
                {
                    //if(len == 0)
                    //   swtich...
                    //else
                    //   switch()
                    //      case...
                    var childrens = childrenGroup.Where(e => !e.Key).First().ToArray();
                    for (int i = 0; i < childrens.Length; i++)
                    {
                        childrens[i] = childrens[i].Clone().WithNotKey();
                    }

                    combin = Expression.IfThenElse(Expression.Equal(len, Expression.Constant(0)), ifZero, BuildMap1FormatterDeserializeCore_SwtichCase(instance, len, key, skip, serializationInfo, childrens, 0, childrens.Length));
                }
            }
            else
            {
                //swtich
                //  case ...
                //  default...
                var down = zeroGroup.Where(e => !e.Key).First().ToArray();
                combin = BuildMap1FormatterDeserializeCore_SwtichCase(instance, len, key, skip, serializationInfo, down, 0, down.Length);
            }

            return Expression.Block(readRaw, combin);
        }

        private static Expression BuildMap1FormatterDeserializeCore_SwtichCase(ParameterExpression instance, ParameterExpression len, ParameterExpression key, LabelTarget skip, ObjectSerializationInfo serializationInfo, BssMapObjMarshal<SerializeMemberInfo>.Entry[] entries, int start, int entryLength)
        {
            //switch(key)
            //   case 0x1:
            //      readValue...
            //   default:
            //      goto skip;
            SwitchCase[] cases = new SwitchCase[entryLength];
            for (int i = 0; i < cases.Length; i++)
            {
                cases[i] = Expression.SwitchCase(BuildMap1FormatterDeserializeCore_ReadValue(instance, len, key, skip, serializationInfo, entries[i + start]), Expression.Constant(entries[i + start].CurrentUInt64Value, typeof(ulong)));
            }
            return Expression.Switch(
                   typeof(void),
                   key,
                   Expression.Goto(skip),
                   null,
                   cases
              );
        }

        private static Expression BuildMap1FormatterDeserializeCore_ReadValue(ParameterExpression instance, ParameterExpression len, ParameterExpression key, LabelTarget skip, ObjectSerializationInfo serializationInfo, BssMapObjMarshal<SerializeMemberInfo>.Entry entry)
        {
            ArrayPack<Expression> ary = new ArrayPack<Expression>(2);

            if (entry.IsKey)
            {
                var memFormatters = serializationInfo.StoreMemberFormatterInstances();
                MemberExpression ele;
                if (entry.Value.Member is FieldInfo f)
                    ele = Expression.Field(instance, f);
                else
                    ele = Expression.Property(instance, (PropertyInfo)entry.Value.Member);

                if (entry.Value.FormatterAttribute != null)
                {
                    //instance.{mem} = formatter.Deserialize(ref reader,option)
                    ary.Add(Expression.Assign(ele, CommonExpressionMeta.Call_FormatterDeserialize(
                       Expression.Convert(Expression.ArrayIndex(Expression.Field(null, memFormatters), Expression.Constant(entry.Value.GetFormatterAttributeIndex())), typeof(IBssomFormatter<>).MakeGenericType(entry.Value.Type)))));
                }
                else
                {
                    if (MapCodeGenInlineTypes.TryGetDeserializeReadExpression(entry.Value.Type, out Expression inlineReadExpr))
                    {
                        //instance.{mem} = reader.Read{TypeName}();
                        ary.Add(Expression.Assign(ele, inlineReadExpr));
                    }
                    else
                    {
                        //instance.{mem} = context.option.Formatter.GetFormatterWithVerify().Serialize(ref writer,instance.{mem}, option)
                        ary.Add(Expression.Assign(ele, CommonExpressionMeta.Call_DeserializeContextOptionResolver_GetFormatterWithVerify_Deserialize(ele.Type)));
                    }
                }
            }
            if (entry.Chidlerns != null)
            {
                ary.Add(BuildMap1FormatterDeserializeCore_ReadBranch(instance, len, key, skip, serializationInfo, entry.Chidlerns, 0, entry.ChidlernLength));
            }
            return Expression.Block(ary);
        }

        #endregion

        #region Size

        public static Expression<Size<T>> BuildSizeLambda<T>(ObjectSerializationInfo serializationInfo, ParameterExpression instance, Expression mapBufferField)
        {
            return Expression.Lambda<Size<T>>(BuildSizeCore(typeof(T), serializationInfo, instance, mapBufferField), CommonExpressionMeta.Par_SizeContext, instance);
        }

        private static Expression BuildSizeCore(Type type, ObjectSerializationInfo serializationInfo, ParameterExpression instance, Expression mapBufferField)
        {
            List<Expression> ary = new List<Expression>();
            LabelTarget returnTarget = Expression.Label(typeof(int), "returnLable");

            if (!type.IsValueType)
            {
                //if (value==null)
                //     goto label: 1;
                ary.Add(CommonExpressionMeta.Block_IfNullSize(instance, type, returnTarget));
            }

            var size = Expression.Variable(typeof(int));
            var memFormatters = serializationInfo.StoreMemberFormatterInstances();
            var mems = serializationInfo.SerializeMemberInfos;
            for (int i = 0; i < mems.Length; i++)
            {
                var mem = mems[i];
                MemberExpression ele;
                if (mem.Member is FieldInfo f)
                    ele = Expression.Field(instance, f);
                else
                    ele = Expression.Property(instance, (PropertyInfo)mem.Member);

                if (mem.FormatterAttribute != null)
                {
                    //size += formatter.Size(instance.{mem}, option) 
                    ary.Add(Expression.AddAssign(size, CommonExpressionMeta.Call_FormatterSize(Expression.Convert(Expression.ArrayIndex(Expression.Field(null, memFormatters), Expression.Constant(mem.GetFormatterAttributeIndex())), typeof(IBssomFormatter<>).MakeGenericType(mem.Type)), ele)));
                }
                else
                {
                    if (MapCodeGenInlineTypes.TryGetSizeExpression(ele, mem.Type, out Expression inlineSize))
                    {
                        //size += ConstantSize
                        ary.Add(Expression.AddAssign(size, inlineSize));
                    }
                    else
                    {
                        //size += option.Formatter.GetFormatterWithVerify().Size(instance.{mem}, option)
                        ary.Add(Expression.AddAssign(size, CommonExpressionMeta.Call_SizeContextOptionResolver_GetFormatterWithVerify_Size(ele)));
                    }
                }
            }

            //size += BssomBinaryPrimitives.BuildInTypeCodeSize;
            ary.Add(Expression.AddAssign(size, Expression.Constant(BssomBinaryPrimitives.BuildInTypeCodeSize)));

            //size += mapBufferSize;
            ary.Add(Expression.AddAssign(size, Expression.ArrayLength(mapBufferField)));

            ary.Add(Expression.Label(returnTarget, size));

            return Expression.Block(new ParameterExpression[] { size }, ary);
        }

        #endregion
    }
}
