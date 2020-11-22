using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.Internal;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Resolvers
{
    public sealed class Array3CodeGenResolver : IFormatterResolver
    {
        internal const string ModuleName = "Bssom.Serializer.Resolvers.Array3CodeGenResolver";
        internal static readonly DynamicFormatterAssembly DynamicAssembly;

        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly Array3CodeGenResolver Instance;

        static Array3CodeGenResolver()
        {
            Instance = new Array3CodeGenResolver();
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
                Formatter = (IBssomFormatter<T>)Activator.CreateInstance(Array3CodeGenResolverBuilder.Build(DynamicAssembly, new ObjectSerializationInfo(t, false)));
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
    internal static class Array3CodeGenResolverBuilder
    {
        public static TypeInfo Build(DynamicFormatterAssembly assembly, ObjectSerializationInfo serializationInfo)
        {
            Type type = serializationInfo.Type;
            TypeBuilder typeBuilder = assembly.DefineFormatterType(type);
            serializationInfo.SerializeMemberInfosOrderByKeyIndex(type);

            MethodBuilder serializeMethod = TypeBuildHelper.DefineSerializeMethod(typeBuilder, type);
            MethodBuilder deserializeMethod = TypeBuildHelper.DefineDeserializeMethod(typeBuilder, type);
            MethodBuilder sizeMethod = TypeBuildHelper.DefineSizeMethod(typeBuilder, type);

            Type delegateCacheType = typeof(Array3DelegateCache<>).MakeGenericType(type);
            delegateCacheType.GetMethod(nameof(Array3DelegateCache<int>.Factory), BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { assembly, serializationInfo });

            TypeBuildHelper.CallSerializeDelegate(serializeMethod, type, delegateCacheType.GetField(nameof(Array3DelegateCache<int>.Serialize)));
            TypeBuildHelper.CallSizeDelegate(sizeMethod, type, delegateCacheType.GetField(nameof(Array3DelegateCache<int>.Size)));
            TypeBuildHelper.CallDeserializeDelegate(deserializeMethod, type, delegateCacheType.GetField(nameof(Array3DelegateCache<int>.Deserialize)));

            return typeBuilder.CreateTypeInfo();
        }

        private static void SerializeMemberInfosOrderByKeyIndex(this ObjectSerializationInfo serializationInfo, Type type)
        {
            if (serializationInfo.SerializeMemberInfos.Length > 0)
            {
                serializationInfo.SerializeMemberInfos = serializationInfo.SerializeMemberInfos.OrderBy(e => e.KeyIndex).ToArray();
                SerializeMemberInfo[] serializeMemberInfos = serializationInfo.SerializeMemberInfos;
                for (int i = 0; i < serializeMemberInfos.Length; i++)
                {
                    var mem = serializeMemberInfos[i];
                    if (!mem.KeyIndexHasValue)
                        throw BssomSerializationTypeFormatterException.Array3MembersMustDefindKeyAttribute(type, mem.Name);

                    if (i != 0 && mem.KeyIndex == serializeMemberInfos[i - 1].KeyIndex)
                        throw BssomSerializationTypeFormatterException.Array3KeyAttributeValueRepeated(type);
                }
            }
        }
    }

    internal static class Array3DynamicExpressionBuild
    {
        #region Serialize

        public static Expression<Serialize<T>> BuildSerializeLambda<T>(ObjectSerializationInfo serializationInfo, ParameterExpression instance)
        {
            return Expression.Lambda<Serialize<T>>(BuildSerializeCore(typeof(T), serializationInfo, instance), CommonExpressionMeta.Par_Writer, CommonExpressionMeta.Par_SerializeContext, instance);
        }

        public unsafe static IntPtr AsPointer<T>(ref T t)
        {
            return new IntPtr(Unsafe.AsPointer(ref t));
        }

        private static Expression BuildSerializeCore(Type type, ObjectSerializationInfo serializationInfo, ParameterExpression instance)
        {
            List<Expression> ary = new List<Expression>();
            LabelTarget returnTarget = Expression.Label(typeof(void), "returnLable");

            if (!type.IsValueType)
            {
                //if (value==null)
                //     writer.WriteNull(); goto label;
                ary.Add(CommonExpressionMeta.Block_IfNullWriteNullWithReturn(instance, type, returnTarget));
            }

            ParameterExpression[] variables = null;
            var keys = serializationInfo.SerializeMemberInfos;
            if (keys.Length == 0)
            {
                //writer.WriteRaw(Array3Cache._EmptyBuffer);
                ary.Add(CommonExpressionMeta.Call_WriteRaw(Expression.Field(null, Array3Cache._EmptyBuffer)));
            }
            else
            {
                int maxLen = keys[keys.Length - 1].KeyIndex + 1;
                Type stackallocBlockType = StackallocBlockProvider.GetOrCreateType(maxLen * sizeof(uint));
                //long position;
                //Block{size} block;
                //IntPtr blockPtr;
                variables = new ParameterExpression[3];
                variables[0] = Expression.Variable(typeof(long), "elementOffPosition");
                variables[1] = Expression.Variable(stackallocBlockType, "block");
                variables[2] = Expression.Variable(typeof(IntPtr),"blockPtr");

                //position = writer.WriteArray3Header(keys.Length);
                ary.Add(Expression.Assign(variables[0], CommonExpressionMeta.Call_WriteArray3Header(maxLen)));
                //block = new Block{size}();
                ary.Add(Expression.Assign(variables[1], Expression.New(stackallocBlockType)));
                //blockPtr = AsPointer(ref block);
                ary.Add(Expression.Assign(variables[2], ExpressionTreeAux.AsPointerExpression(variables[1])));

                //0,3,5  --> maxLen = 6
                FieldInfo memFormatters = serializationInfo.StoreMemberFormatterInstances();
                int realIndex = 0;


                for (int i = 0; i < maxLen; i++)
                {
                    //StackallocBlockHelper.WriteUInt(blockPtr, 0, (uint)(writer.Position - position));
                    ary.Add(Expression.Call(null, StackallocBlockHelper._WriteUIntMethodInfo, variables[2], Expression.Constant(i), Expression.Convert(Expression.Subtract(CommonExpressionMeta.Field_WriterPos, variables[0]), typeof(uint))));

                    if (keys[realIndex].KeyIndex != i)
                    {
                        //WriteNull()
                        ary.Add(CommonExpressionMeta.Call_Writer_WriteNull);
                    }
                    else
                    {
                        //Writer(mem.Value)
                        ary.Add(SpecialCodeGenExpression.WriteValues(keys[realIndex], instance, memFormatters));
                        realIndex++;
                    }
                }

                //writer.WriteBackArray3Header(blockPtr)
                ary.Add(CommonExpressionMeta.Call_WriteBackArray3Header(variables[0], variables[2], maxLen));
            }

            ary.Add(Expression.Label(returnTarget));

            if (variables != null)
                return Expression.Block(variables, ary);
            return Expression.Block(ary);
        }

        #endregion

        #region Deserialize

        public static Expression<Deserialize<T>> BuildDeserializeLambda<T>(ObjectSerializationInfo serializationInfo)
        {
            return Expression.Lambda<Deserialize<T>>(BuildDeserializeCore(typeof(T), serializationInfo), CommonExpressionMeta.Par_Reader, CommonExpressionMeta.Par_DeserializeContext);
        }

        private static Expression BuildDeserializeCore(Type t, ObjectSerializationInfo serializationInfo)
        {
            List<Expression> ary = new List<Expression>();
            LabelTarget returnTarget = Expression.Label(t, "returnLable");
            //int num;
            ParameterExpression num = Expression.Variable(typeof(int), "num");
            //int for-i;
            ParameterExpression forVariable = Expression.Variable(typeof(int), "i");

            //context.option.Security.DepthStep(ref reader);
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Option_Security_DepthStep);

            //if(reader.TryReadNullWithEnsureBuildInType(BssomType.Array3))
            //      return default(t);
            ary.Add(Expression.IfThen(CommonExpressionMeta.Call_Reader_TryReadNullWithEnsureBuildInType(BssomType.Array3),
                Expression.Return(returnTarget, Expression.Default(t))));

            //T t = new T();
            ParameterExpression instance = Expression.Parameter(t, "instance");
            if (serializationInfo.IsDefaultNoArgsCtor)
            {
                ary.Add(Expression.Assign(instance, Expression.New(t)));
            }
            else
            {
                ParameterInfo[] parInfos = serializationInfo.BestmatchConstructor.GetParameters();
                Expression[] pars = new Expression[parInfos.Length];
                if (serializationInfo.ConstructorParametersIsDefaultValue)
                {
                    for (int i = 0; i < parInfos.Length; i++)
                    {
                        pars[i] = Expression.Default(parInfos[i].ParameterType);
                    }
                    ary.Add(Expression.Assign(instance, Expression.New(serializationInfo.BestmatchConstructor, pars)));
                }
                else
                {
                    object[] cps = serializationInfo.ConstructorParameters;
                    for (int i = 0; i < parInfos.Length; i++)
                    {
                        pars[i] = Expression.Constant(cps[i], parInfos[i].ParameterType);
                    }
                    ary.Add(Expression.Assign(instance, Expression.New(serializationInfo.BestmatchConstructor, pars)));
                }
            }

            //reader.SkipVariableNumber()
            ary.Add(CommonExpressionMeta.Call_Reader_SkipVariableNumber);
            //num = reader.ReadVariableNumber()
            ary.Add(Expression.Assign(num, CommonExpressionMeta.Call_Reader_ReadVariableNumber));
            //i = 0;
            ary.Add(Expression.Assign(forVariable, Expression.Constant(0)));

            var members = serializationInfo.SerializeMemberInfos;
            if (members.Length > 0)
            {
                //reader.Buffer.Seek(offsetSegment, Current)
                ary.Add(CommonExpressionMeta.Call_Reader_BufferSeek(Expression.Convert(Expression.Multiply(num, Expression.Constant(BssomBinaryPrimitives.FixUInt32NumberSize)), typeof(Int64)), BssomSeekOrgin.Current));
                //switch(i)
                //  case 0: instance.Key0 = readValue();
                //  case 3: instance.Key1 = readValue();
                //  case 5: instance.Key2 = readValue();
                //  default: skipObj();
                FieldInfo memFormatters = serializationInfo.StoreMemberFormatterInstances();
                SwitchCase[] switchCases = new SwitchCase[members.Length];
                for (int i = 0; i < members.Length; i++)
                {
                    switchCases[i] = Expression.SwitchCase(SpecialCodeGenExpression.ReadValues(members[i], instance, memFormatters), Expression.Constant(members[i].KeyIndex));
                }
                Expression content = Expression.Switch(
                    typeof(void),
                    forVariable,
                    CommonExpressionMeta.Call_Reader_SkipObject,
                    null,
                    switchCases
                    );

                ary.Add(For(forVariable, Expression.LessThan(forVariable, num), Expression.Assign(forVariable, Expression.Add(forVariable, Expression.Constant(1))), content));
            }
            //context.Depth--;
            ary.Add(CommonExpressionMeta.Call_DeserializeContext_Depth_DecrementAssign);

            ary.Add(Expression.Return(returnTarget, instance));

            ary.Add(Expression.Label(returnTarget, instance));

            return Expression.Block(new ParameterExpression[] { instance, num, forVariable, }, ary);
        }

        private static Expression For(ParameterExpression loopVar, Expression condition, Expression increment, Expression loopContent)
        {
            var initAssign = Expression.Assign(loopVar, Expression.Constant(0));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(
                initAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        condition,
                        Expression.Block(
                            loopContent,
                            increment
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        #endregion

        #region Size

        public static Expression<Size<T>> BuildSizeLambda<T>(ObjectSerializationInfo serializationInfo, ParameterExpression instance)
        {
            return Expression.Lambda<Size<T>>(BuildSizeCore(typeof(T), serializationInfo, instance), CommonExpressionMeta.Par_SizeContext, instance);
        }

        private static Expression BuildSizeCore(Type type, ObjectSerializationInfo serializationInfo, ParameterExpression instance)
        {
            List<Expression> ary = new List<Expression>();
            LabelTarget returnTarget = Expression.Label(typeof(int), "returnLable");

            if (!type.IsValueType)
            {
                //if (value==null)
                //     goto label: 1;
                ary.Add(CommonExpressionMeta.Block_IfNullSize(instance, type, returnTarget));
            }

            ParameterExpression size = Expression.Variable(typeof(int));
            SerializeMemberInfo[] mems = serializationInfo.SerializeMemberInfos;
            if (mems.Length == 0)
            {
                ary.Add(Expression.Assign(size, Expression.Constant(Array3Cache.Empty.Length)));
            }
            else
            {
                int maxLen = mems[mems.Length - 1].KeyIndex + 1;
                ary.Add(Expression.Assign(size, Expression.Constant(BssomBinaryPrimitives.Array3HeaderSize(maxLen))));

                FieldInfo memFormatters = serializationInfo.StoreMemberFormatterInstances();
                int nullNumber = 0;
                int realIndex = 0;
                for (int i = 0; i < maxLen; i++)
                {
                    if (mems[realIndex].KeyIndex != i)
                    {
                        nullNumber++;
                    }
                    else
                    {
                        //Size(mem.Value)
                        ary.Add(SpecialCodeGenExpression.SizeValues(mems[realIndex], instance, size, memFormatters));
                        realIndex++;
                    }
                }
                if (nullNumber > 0)
                    ary.Add(Expression.AddAssign(size, Expression.Constant(nullNumber * BssomBinaryPrimitives.NullSize)));
            }

            ary.Add(Expression.Label(returnTarget, size));

            return Expression.Block(new ParameterExpression[] { size }, ary);
        }

        #endregion
    }

    internal static class Array3DelegateCache<T>
    {
        public static Serialize<T> Serialize;
        public static Deserialize<T> Deserialize;
        public static Size<T> Size;

        internal static void Factory(DynamicFormatterAssembly assembly, ObjectSerializationInfo objectSerializationInfo)
        {
            ParameterExpression instance = Expression.Parameter(objectSerializationInfo.Type, "value");

            Expression<Serialize<T>> serializeExpression = Array3DynamicExpressionBuild.BuildSerializeLambda<T>(objectSerializationInfo, instance);
            Expression<Size<T>> sizeExpression = Array3DynamicExpressionBuild.BuildSizeLambda<T>(objectSerializationInfo, instance);
            Expression<Deserialize<T>> deserializeExpression = Array3DynamicExpressionBuild.BuildDeserializeLambda<T>(objectSerializationInfo);

            Serialize = serializeExpression.Compile();
            Size = sizeExpression.Compile();
            Deserialize = deserializeExpression.Compile();

#if NETFRAMEWORK
            TypeBuilder typeBuilder = assembly.DefineFormatterDelegateType(objectSerializationInfo.Type);
            MethodBuilder serializeDelegate = TypeBuildHelper.DefineSerializeDelegate(typeBuilder, typeof(T));
            serializeExpression.CompileToMethod(serializeDelegate);
            MethodBuilder sizeDelegate = TypeBuildHelper.DefineSizeDelegate(typeBuilder, typeof(T));
            sizeExpression.CompileToMethod(sizeDelegate);
            MethodBuilder deserializeDelegate = TypeBuildHelper.DefineDeserializeDelegate(typeBuilder, typeof(T));
            deserializeExpression.CompileToMethod(deserializeDelegate);
            typeBuilder.CreateTypeInfo();
#endif
        }
    }

    internal static class Array3Cache
    {
        public static readonly FieldInfo _EmptyBuffer = typeof(Array3Cache).GetField(nameof(Empty));

        public static byte[] Empty;

        static Array3Cache()
        {

            ExpandableBufferWriter bw = ExpandableBufferWriter.CreateTemporary();
            BssomWriter cw = new BssomWriter(bw);
            cw.WriteBuildInType(BssomType.Array3);
            cw.WriteVariableNumber(BssomBinaryPrimitives.FixUInt32NumberSize);//len
            cw.WriteUInt32FixNumber(0);//count
            Empty = bw.GetBufferedArray();

        }
    }
}
