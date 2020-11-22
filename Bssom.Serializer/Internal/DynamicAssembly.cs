//using System.Runtime.CompilerServices;

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Bssom.Serializer.Internal
{
    internal class DynamicAssembly
    {
        private readonly string moduleName;
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;

        private readonly object gate = new object();

        public DynamicAssembly(string moduleName)
        {
#if NETFRAMEWORK 
            AssemblyBuilderAccess builderAccess = AssemblyBuilderAccess.RunAndSave;
#else
            AssemblyBuilderAccess builderAccess = AssemblyBuilderAccess.Run;
#endif
            this.moduleName = moduleName;
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(moduleName), builderAccess);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName + ".dll");
        }

        public TypeBuilder DefineStructType(string name, TypeAttributes attr, PackingSize packingSize, int typesize)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr, typeof(ValueType), packingSize, typesize);
            }
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            lock (gate)
            {
                return moduleBuilder.DefineType(name, attr, parent, interfaces);
            }
        }

        public static void VerifyTypeIsPublic(Type type)
        {
            if (!(type.IsPublic || type.IsNestedPublic))
            {
                throw BssomSerializationTypeFormatterException.BuildNoPublicDynamicType(type);
            }
        }

#if NETFRAMEWORK 

        public AssemblyBuilder Save()
        {
            assemblyBuilder.Save(moduleName + ".dll");
            return assemblyBuilder;
        }

#endif
    }

    internal class DynamicFormatterAssembly : DynamicAssembly
    {
        private int nameSequence = 0;
        private static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        public DynamicFormatterAssembly(string moduleName) : base(moduleName)
        {
        }

        public TypeBuilder DefineCollectionFormatterType(Type type, Type elementType)
        {
            VerifyTypeIsPublic(type);

            string pre = "Array2.";
            if (Array1FormatterHelper.IsArray1Type(elementType))
            {
                pre = "Array1.";
            }

            Type formatterType = typeof(IBssomFormatter<>).MakeGenericType(type);
            TypeBuilder typeBuilder = DefineType("Bssom.Formatters." + pre + SubtractFullNameRegex.Replace(type.FullName, string.Empty).Replace(".", "_") + "Formatter" + Interlocked.Increment(ref nameSequence), TypeAttributes.NotPublic | TypeAttributes.Sealed, null, new[] { formatterType });
            return typeBuilder;
        }

        public TypeBuilder DefineFormatterType(Type type)
        {
            VerifyTypeIsPublic(type);

            Type formatterType = typeof(IBssomFormatter<>).MakeGenericType(type);
            TypeBuilder typeBuilder = DefineType("Bssom.Formatters." + SubtractFullNameRegex.Replace(type.FullName, string.Empty).Replace(".", "_") + "Formatter" + Interlocked.Increment(ref nameSequence), TypeAttributes.NotPublic | TypeAttributes.Sealed, null, new[] { formatterType });
            return typeBuilder;
        }

        public TypeBuilder DefineFormatterDelegateType(Type type)
        {
            TypeBuilder typeBuilder = DefineType("Bssom.Formatters." + SubtractFullNameRegex.Replace(type.FullName, string.Empty).Replace(".", "_") + "FormatterDelegate" + Interlocked.Increment(ref nameSequence), TypeAttributes.NotPublic | TypeAttributes.Sealed, null, null);
            return typeBuilder;
        }
    }

    internal class DynamicInterfaceImpAssembly : DynamicAssembly
    {
        private int nameSequence = 0;
        private static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        public DynamicInterfaceImpAssembly(string moduleName) : base(moduleName)
        {
        }

        public TypeBuilder DefineInterfaceImpType(Type interfaceType)
        {
            VerifyTypeIsPublic(interfaceType);

            TypeBuilder typeBuilder = DefineType("Bssom.DynamicInterfaceImp." + SubtractFullNameRegex.Replace(interfaceType.FullName, string.Empty).Replace(".", "_") + Interlocked.Increment(ref nameSequence), TypeAttributes.Public | TypeAttributes.Sealed, null, new[] { interfaceType });
            return typeBuilder;
        }
    }

    internal class DynamicStackallocBlocksAssembly : DynamicAssembly
    {
        private int nameSequence = 0;
        private static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        public DynamicStackallocBlocksAssembly(string moduleName) : base(moduleName)
        {
        }

        public TypeBuilder DefineBlockType(int blockSize)
        {
            TypeBuilder typeBuilder = DefineStructType($"Bssom.DynamicStackallocBlocks.Block{blockSize}", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.ExplicitLayout | TypeAttributes.BeforeFieldInit, PackingSize.Unspecified, blockSize);
            return typeBuilder;
        }
    }
}
