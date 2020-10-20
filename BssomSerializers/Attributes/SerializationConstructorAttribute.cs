//using System.Runtime.CompilerServices;
using BssomSerializers.Resolver;
using System;

namespace BssomSerializers
{
    /// <summary>
    /// <para>指示<see cref="MapCodeGenResolver"/>在反序列化该对象时使用指定的构造函数</para>
    /// <para>Instructs the <see cref="MapCodeGenResolver"/> to use the specified constructor when deserializing that object.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class SerializationConstructorAttribute : Attribute
    {
        internal object[] _paras { get; set; }

        /// <summary>
        ///     Structural aliases
        /// </summary>
        /// <param name="name"></param>
        public SerializationConstructorAttribute(params object[] paras)
        {
            _paras = paras;
        }
    }
}
