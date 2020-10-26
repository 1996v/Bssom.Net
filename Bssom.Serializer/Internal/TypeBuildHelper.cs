//using System.Runtime.CompilerServices;

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Bssom.Serializer.Internal
{
    internal static class TypeBuildHelper
    {
        public static MethodBuilder DefineSerializeMethod(TypeBuilder typeBuilder, Type type, string methodName = null)
        {
            MethodBuilder method = typeBuilder.DefineMethod(
              methodName ?? nameof(IBssomFormatter<int>.Serialize),
               MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
               returnType: null,
               parameterTypes: new Type[] { typeof(BssomWriter).MakeByRefType(), typeof(BssomSerializeContext).MakeByRefType(), type });
            method.DefineParameter(1, ParameterAttributes.None, "writer");
            method.DefineParameter(2, ParameterAttributes.None, "context");
            method.DefineParameter(3, ParameterAttributes.None, "value");
            return method;
        }

        public static MethodBuilder DefineSerializeDelegate(TypeBuilder typeBuilder, Type type, string methodName = null)
        {
            MethodBuilder method = typeBuilder.DefineMethod(
              methodName ?? nameof(IBssomFormatter<int>.Serialize),
               MethodAttributes.Public | MethodAttributes.Static,
               returnType: null,
               parameterTypes: new Type[] { typeof(BssomWriter).MakeByRefType(), typeof(BssomSerializeContext).MakeByRefType(), type });
            method.DefineParameter(1, ParameterAttributes.None, "writer");
            method.DefineParameter(2, ParameterAttributes.None, "context");
            method.DefineParameter(3, ParameterAttributes.None, "value");
            return method;
        }

        public static void CallOneMethodInSerialize(MethodBuilder serialize, MethodInfo method)
        {
            ILGenerator il = serialize.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ret);
        }

        public static MethodBuilder DefineDeserializeMethod(TypeBuilder typeBuilder, Type t)
        {
            MethodBuilder method = typeBuilder.DefineMethod(
               nameof(IBssomFormatter<int>.Deserialize),
               MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
               returnType: t,
               parameterTypes: new Type[] { typeof(BssomReader).MakeByRefType(), typeof(BssomDeserializeContext).MakeByRefType() });
            method.DefineParameter(1, ParameterAttributes.None, "reader");
            method.DefineParameter(2, ParameterAttributes.None, "context");
            return method;
        }

        public static void CallOneMethodInDeserialize(MethodBuilder deserialize, MethodInfo method)
        {
            ILGenerator il = deserialize.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ret);
        }

        public static void CallOneStaticFieldMethodInDeserialize(MethodBuilder deserialize, FieldInfo fieldInfo, MethodInfo method)
        {
            ILGenerator il = deserialize.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, fieldInfo);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, method);
            il.Emit(OpCodes.Ret);
        }

        public static void CallDeserializeDelegate(MethodBuilder deserializeMethod, Type genericType, FieldInfo fieldInfo)
        {
            ILGenerator il = deserializeMethod.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, fieldInfo);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, typeof(Deserialize<>).MakeGenericType(genericType).GetMethod(nameof(Deserialize<int>.Invoke)));
            il.Emit(OpCodes.Ret);
        }

        public static void CallSerializeDelegate(MethodBuilder serializeMethod, Type genericType, FieldInfo fieldInfo)
        {
            ILGenerator il = serializeMethod.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, fieldInfo);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldarg_3);
            il.Emit(OpCodes.Call, typeof(Serialize<>).MakeGenericType(genericType).GetMethod(nameof(Serialize<int>.Invoke)));
            il.Emit(OpCodes.Ret);
        }

        public static void CallSizeDelegate(MethodBuilder serializeMethod, Type genericType, FieldInfo fieldInfo)
        {
            ILGenerator il = serializeMethod.GetILGenerator();
            il.Emit(OpCodes.Ldsfld, fieldInfo);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, typeof(Size<>).MakeGenericType(genericType).GetMethod(nameof(Size<int>.Invoke)));
            il.Emit(OpCodes.Ret);
        }

        public static MethodBuilder DefineSizeMethod(TypeBuilder typeBuilder, Type type)
        {
            MethodBuilder method = typeBuilder.DefineMethod(
              nameof(IBssomFormatter<int>.Size),
              MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
              returnType: typeof(int),
              parameterTypes: new Type[] { typeof(BssomSizeContext).MakeByRefType(), type });
            method.DefineParameter(1, ParameterAttributes.None, "context");
            method.DefineParameter(2, ParameterAttributes.None, "value");
            return method;
        }

        public static MethodBuilder DefineSizeDelegate(TypeBuilder typeBuilder, Type type)
        {
            MethodBuilder method = typeBuilder.DefineMethod(
              nameof(IBssomFormatter<int>.Size),
                MethodAttributes.Public | MethodAttributes.Static,
              returnType: typeof(int),
              parameterTypes: new Type[] { typeof(BssomSizeContext).MakeByRefType(), type });
            method.DefineParameter(1, ParameterAttributes.None, "context");
            method.DefineParameter(2, ParameterAttributes.None, "value");
            return method;
        }

        public static MethodBuilder DefineMap1FormatterDeserializeDelegate(TypeBuilder typeBuilder, Type type)
        {
            MethodBuilder method = typeBuilder.DefineMethod(
              nameof(Map1FormatterDeserialize<int>),
                MethodAttributes.Public | MethodAttributes.Static,
              returnType: typeof(void),
              parameterTypes: new Type[] { typeof(BssomSerializeContext).MakeByRefType(), typeof(BssomReader).MakeByRefType(), type.MakeByRefType() });
            method.DefineParameter(1, ParameterAttributes.None, "context");
            method.DefineParameter(2, ParameterAttributes.None, "reader");
            method.DefineParameter(3, ParameterAttributes.None, "value");
            return method;
        }

        public static void CallOneMethodInSize(MethodBuilder serialize, MethodInfo method)
        {
            ILGenerator il = serialize.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, method);
            il.Emit(OpCodes.Ret);
        }
    }
}
