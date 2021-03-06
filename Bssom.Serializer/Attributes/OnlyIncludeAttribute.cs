﻿//using System.Runtime.CompilerServices;
using Bssom.Serializer.Resolvers;
using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>指示<see cref="MapCodeGenResolver"/>在序列化和反序列化时仅包含此元素</para>
    /// <para>Indicates that <see cref="MapCodeGenResolver" /> only contains this element when serializing and deserializing</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OnlyIncludeAttribute : Attribute
    {
    }
}
