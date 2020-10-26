//using System.Runtime.CompilerServices;
using Bssom.Serializer.Resolvers;
using System;

namespace Bssom.Serializer
{
    /// <summary>
    ///<para>指示<see cref="MapCodeGenResolver"/>在序列化和反序列化时忽略被标记的元素</para>
    ///<para>Instructs <see cref="MapCodeGenResolver" /> to ignore marked elements during serialization and deserialization</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreKeyAttribute : Attribute
    {
    }
}
