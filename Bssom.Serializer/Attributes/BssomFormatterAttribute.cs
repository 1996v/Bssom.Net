//using System.Runtime.CompilerServices;

using Bssom.Serializer.Resolvers;
using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>指示<see cref="AttributeFormatterResolver"/>和<see cref="MapCodeGenResolver"/>在序列化和反序列化时为所标记的对象设置一个特殊的格式化类型</para>
    /// <para>Indicate <see cref="AttributeFormatterResolver" /> and <see cref="MapCodeGenResolver" /> to set the special format type of the marked object during serialization and deserialization</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class BssomFormatterAttribute : Attribute
    {
        public Type FormatterType { get; internal set; }

        public object[] Arguments { get; private set; }

        public BssomFormatterAttribute(Type formatterType)
        {
            FormatterType = formatterType;
        }

        public BssomFormatterAttribute(Type formatterType, params object[] arguments)
        {
            FormatterType = formatterType;
            Arguments = arguments;
        }
    }
}
