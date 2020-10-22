//using System.Runtime.CompilerServices;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Bssom.Serializer.Internal
{
    internal static class EmitHelper
    {
        public static void EmitDefault(this ILGenerator il, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.DateTime:
                    il.Emit(OpCodes.Ldsfld, typeof(DateTime).GetField(nameof(DateTime.MinValue)));
                    return;
                case TypeCode.Decimal:
                    il.Emit(OpCodes.Ldsfld, typeof(decimal).GetField(nameof(decimal.Zero)));
                    return;
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Conv_I8);
                    return;

                case TypeCode.Single:
                    il.Emit(OpCodes.Ldc_R4, default(float));
                    return;

                case TypeCode.Double:
                    il.Emit(OpCodes.Ldc_R8, default(double));
                    return;
            }

            if (type.IsValueType)
            {
                LocalBuilder lb = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldloca, lb);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc, lb);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
        }

        public static void EmitStloc(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (index <= 255)
                    {
                        il.Emit(OpCodes.Stloc_S, (byte)index);
                    }
                    else
                    {
                        il.Emit(OpCodes.Stloc, (short)index);
                    }

                    break;
            }
        }

        public static void EmitLdloc(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index <= 255)
                    {
                        il.Emit(OpCodes.Ldloc_S, (byte)index);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc, (short)index);
                    }

                    break;
            }
        }

        public static void EmitLdloca(this ILGenerator il, int index)
        {
            if (index <= 255)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)index);
            }
            else
            {
                il.Emit(OpCodes.Ldloca, (short)index);
            }
        }

        public static void EmitLdarg(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= 255)
                    {
                        il.Emit(OpCodes.Ldarg_S, (byte)index);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg, index);
                    }

                    break;
            }
        }

        public static void EmitLdc_I4(this ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }

                    break;
            }
        }

        public static void EmitObjectConvertTo(this ILGenerator il, Type target)
        {
            if (target != typeof(object))
            {
                if (target.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, target);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, target);
                }
            }
        }

        public static void EmitGetPropertyOrField(this ILGenerator il, MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                il.EmitCall(property.GetGetMethod(true));
            }
            else
            {
                il.Emit(OpCodes.Ldfld, (FieldInfo)member);
            }
        }

        public static void EmitSetPropertyOrField(this ILGenerator il, MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                il.EmitCall(property.GetSetMethod(true));
            }
            else
            {
                il.Emit(OpCodes.Stfld, (FieldInfo)member);
            }
        }

        public static void EmitCall(this ILGenerator il, MethodInfo methodInfo)
        {
            if (methodInfo.IsFinal || !methodInfo.IsVirtual)
            {
                il.Emit(OpCodes.Call, methodInfo);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, methodInfo);
            }
        }
    }


}
