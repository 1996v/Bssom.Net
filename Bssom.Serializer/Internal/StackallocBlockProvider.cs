using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.Internal
{
    internal static class StackallocBlockProvider
    {
        internal const string ModuleName = "Bssom.StackallocBlocks";

        private static Dictionary<int, Type> dynamicGenerateBlockTypes;
        internal static readonly DynamicStackallocBlocksAssembly DynamicAssembly;

        static StackallocBlockProvider()
        {
            dynamicGenerateBlockTypes = new Dictionary<int, Type>();
            DynamicAssembly = new DynamicStackallocBlocksAssembly(ModuleName);
        }

        public static Type GetOrCreateType(int blockSize)
        {
            lock (dynamicGenerateBlockTypes)
            {
                if (!dynamicGenerateBlockTypes.TryGetValue(blockSize, out Type blockType))
                {
                    TypeBuilder blockTypeBuilder = DynamicAssembly.DefineBlockType(blockSize);
                    blockType = blockTypeBuilder.CreateTypeInfo();
                    dynamicGenerateBlockTypes.Add(blockSize, blockType);
                }
                return blockType;
            }
        }
    }

    internal static class StackallocBlockHelper
    {
        public readonly static MethodInfo _WriteUIntMethodInfo = typeof(StackallocBlockHelper).GetMethod(nameof(WriteUInt));

        public static unsafe void WriteUInt(void* ptr, int uintIndex, uint uintValue)
        {
            Unsafe.Add(ref Unsafe.AsRef<uint>(ptr), uintIndex) = uintValue;
        }
    }
}
