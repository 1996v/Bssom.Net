using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
namespace Bssom.Serializer.Internal
{
    internal static class InterfaceImplementation
    {
        private const string ModuleName = "Bssom.DynamicInterfaceImp";
        private static readonly DynamicInterfaceImpAssembly ImpAssembly;

        static InterfaceImplementation()
        {
            ImpAssembly = new DynamicInterfaceImpAssembly(ModuleName);
        }

        public static Type CreateType(Type interfaceType)
        {
            try
            {
                TypeBuilder typeBuilder = ImpAssembly.DefineInterfaceImpType(interfaceType);

                List<MemberInfo> allMembers = interfaceType.GetAllInterfaceMembers();

                List<MethodInfo> propertyInfos = new List<MethodInfo>();

                foreach (PropertyInfo prop in allMembers.OfType<PropertyInfo>())
                {
                    Type propType = prop.PropertyType;

                    PropertyBuilder propBuilder = typeBuilder.DefineProperty(prop.Name, prop.Attributes, propType, Type.EmptyTypes);

                    MethodInfo iGetter = prop.GetMethod;
                    MethodInfo iSetter = prop.SetMethod;
                    if (iGetter != null)
                    {
                        propertyInfos.Add(iGetter);
                    }

                    if (iSetter != null)
                    {
                        propertyInfos.Add(iSetter);
                    }

                    if (prop.Name == "Item")
                    {
                        if (iGetter != null)
                        {
                            MethodAttributes accessor = iGetter.Attributes;
                            accessor &= ~MethodAttributes.Abstract;
                            MethodBuilder methBuilder = typeBuilder.DefineMethod(iGetter.Name, accessor, iGetter.ReturnType, iGetter.GetParameters().Select(e => e.ParameterType).ToArray());
                            ILGenerator il = methBuilder.GetILGenerator();
                            il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                            il.Emit(OpCodes.Throw);
                            propBuilder.SetGetMethod(methBuilder);
                        }
                        if (iSetter != null)
                        {
                            MethodAttributes accessor = iSetter.Attributes;
                            accessor &= ~MethodAttributes.Abstract;
                            MethodBuilder methBuilder = typeBuilder.DefineMethod(iSetter.Name, accessor, iSetter.ReturnType, iSetter.GetParameters().Select(e => e.ParameterType).ToArray());
                            ILGenerator il = methBuilder.GetILGenerator();
                            il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                            il.Emit(OpCodes.Throw);
                            propBuilder.SetSetMethod(methBuilder);
                        }
                        continue;
                    }


                    Func<FieldInfo> getBackingField;
                    {
                        FieldInfo backingField = null;
                        getBackingField =
                            () =>
                            {
                                if (backingField == null)
                                {
                                    backingField = typeBuilder.DefineField("_" + prop.Name + "_" + Guid.NewGuid(), propType, FieldAttributes.Private);
                                }

                                return backingField;
                            };
                    }

                    if (iGetter != null)
                    {
                        MethodAttributes accessor = iGetter.Attributes;
                        accessor &= ~MethodAttributes.Abstract;

                        MethodBuilder methBuilder = typeBuilder.DefineMethod(iGetter.Name, accessor, propType, Type.EmptyTypes);
                        ILGenerator il = methBuilder.GetILGenerator();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, getBackingField());
                        il.Emit(OpCodes.Ret);
                        propBuilder.SetGetMethod(methBuilder);
                    }

                    if (iGetter != null || iSetter != null)
                    {
                        MethodAttributes accessor = iSetter != null ? iSetter.Attributes : MethodAttributes.Private;
                        string name = iSetter != null ? iSetter.Name : "set_" + prop.Name;

                        accessor &= ~MethodAttributes.Abstract;

                        MethodBuilder methBuilder = typeBuilder.DefineMethod(name, accessor, typeof(void), new[] { propType });
                        ILGenerator il = methBuilder.GetILGenerator();

                        if (iGetter != null)
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Ldarg_1);
                            il.Emit(OpCodes.Stfld, getBackingField());
                            il.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ret);
                        }

                        propBuilder.SetSetMethod(methBuilder);
                    }
                }

                foreach (MethodInfo method in allMembers.OfType<MethodInfo>().Except(propertyInfos))
                {
                    MethodBuilder methBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, method.ReturnType, method.GetParameters().Select(e => e.ParameterType).ToArray());
                    if (method.IsGenericMethod)
                    {
                        methBuilder.DefineGenericParameters(method.GetGenericArguments().Select(e => e.Name).ToArray());
                    }
                    ILGenerator il = methBuilder.GetILGenerator();
                    il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                    il.Emit(OpCodes.Throw);

                    typeBuilder.DefineMethodOverride(methBuilder, method);
                }

                return typeBuilder.CreateTypeInfo();
            }
            catch
            {
                throw BssomSerializationTypeFormatterException.UnsupportedType(interfaceType);
            }
        }
    }
}
