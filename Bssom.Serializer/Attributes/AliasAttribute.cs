//using System.Runtime.CompilerServices;

using Bssom.Serializer.Resolvers;
using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>指示<see cref="MapCodeGenResolver"/>在序列化和反序列化期间对字段或属性使用指定的名称 </para>
    /// <para>Instruct <see cref="MapCodeGenResolver"/> to use the specified name for the field or property during serialization and deserialization</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AliasAttribute : Attribute
    {
        public string Name { get; }

        public AliasAttribute(string name)
        {
            Name = name;
        }
    }
}
