using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Resolver;
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
                var typeBuilder = ImpAssembly.DefineInterfaceImpType(interfaceType);

                var allMembers = interfaceType.GetAllInterfaceMembers();

                List<MethodInfo> propertyInfos = new List<MethodInfo>();

                foreach (var prop in allMembers.OfType<PropertyInfo>())
                {
                    var propType = prop.PropertyType;

                    var propBuilder = typeBuilder.DefineProperty(prop.Name, prop.Attributes, propType, Type.EmptyTypes);

                    var iGetter = prop.GetMethod;
                    var iSetter = prop.SetMethod;
                    if (iGetter != null)
                        propertyInfos.Add(iGetter);
                    if (iSetter != null)
                        propertyInfos.Add(iSetter);

                    if (prop.Name == "Item")
                    {
                        if (iGetter != null)
                        {
                            var accessor = iGetter.Attributes;
                            accessor &= ~MethodAttributes.Abstract;
                            var methBuilder = typeBuilder.DefineMethod(iGetter.Name, accessor, iGetter.ReturnType, iGetter.GetParameters().Select(e => e.ParameterType).ToArray());
                            var il = methBuilder.GetILGenerator();
                            il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructors()[0]);
                            il.Emit(OpCodes.Throw);
                            propBuilder.SetGetMethod(methBuilder);
                        }
                        if (iSetter != null)
                        {
                            var accessor = iSetter.Attributes;
                            accessor &= ~MethodAttributes.Abstract;
                            var methBuilder = typeBuilder.DefineMethod(iSetter.Name, accessor, iSetter.ReturnType, iSetter.GetParameters().Select(e => e.ParameterType).ToArray());
                            var il = methBuilder.GetILGenerator();
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
                        var accessor = iGetter.Attributes;
                        accessor &= ~MethodAttributes.Abstract;

                        var methBuilder = typeBuilder.DefineMethod(iGetter.Name, accessor, propType, Type.EmptyTypes);
                        var il = methBuilder.GetILGenerator();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, getBackingField());
                        il.Emit(OpCodes.Ret);
                        propBuilder.SetGetMethod(methBuilder);
                    }

                    if (iGetter != null || iSetter != null)
                    {
                        var accessor = iSetter != null ? iSetter.Attributes : MethodAttributes.Private;
                        var name = iSetter != null ? iSetter.Name : "set_" + prop.Name;

                        accessor &= ~MethodAttributes.Abstract;

                        var methBuilder = typeBuilder.DefineMethod(name, accessor, typeof(void), new[] { propType });
                        var il = methBuilder.GetILGenerator();

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

                foreach (var method in allMembers.OfType<MethodInfo>().Except(propertyInfos))
                {
                    var methBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, method.ReturnType, method.GetParameters().Select(e => e.ParameterType).ToArray());
                    if (method.IsGenericMethod)
                    {
                        methBuilder.DefineGenericParameters(method.GetGenericArguments().Select(e => e.Name).ToArray());
                    }
                    var il = methBuilder.GetILGenerator();
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
