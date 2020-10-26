using Bssom.Serializer.Resolvers;
using System;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>运行<see cref="BssomSerializer"/>期间所使用的配置</para>
    /// <para>The configuration used during running <see cref="BssomSerializer"/></para>
    /// </summary>
    public class BssomSerializerOptions
    {
        /// <summary>
        /// <para>使用了<see cref="CompositedResolver"/>的默认配置集</para>
        /// <para>The default configuration set of <see cref ="CompositedResolver"/> is used</para>
        /// </summary>
        public static BssomSerializerOptions Default = new BssomSerializerOptions();

        protected BssomSerializerOptions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializerOptions"/> struct.
        /// </summary>
        protected BssomSerializerOptions(BssomSerializerOptions copyFrom)
        {
            if (copyFrom == null)
            {
                throw new ArgumentNullException(nameof(copyFrom));
            }

            Security = copyFrom.Security;
            FormatterResolver = copyFrom.FormatterResolver;
            IsPriorityToDeserializeObjectAsBssomValue = copyFrom.IsPriorityToDeserializeObjectAsBssomValue;
            IsUseStandardDateTime = copyFrom.IsUseStandardDateTime;
            IDictionaryIsSerializeMap1Type = copyFrom.IDictionaryIsSerializeMap1Type;
        }

        /// <summary>
        /// <para>获取用于序列化期间的安全相关选项</para>
        /// <para>Get security-related options used during serialization</para>
        /// </summary>
        public BssomSecurity Security { get; private set; } = BssomSecurity.TrustedData;

        /// <summary>
        /// <para>获取用于序列化期间的类型解析器</para>
        /// <para>Gets the type resolver for use during serialization</para>
        /// </summary>
        public IFormatterResolver FormatterResolver { get; private set; } = CompositedResolver.Instance;

        /// <summary>
        /// <para>在反序列化时是否优先将Object对象默认解析成BssomValue</para>
        /// <para>Whether to preferentially parse Object objects into BssomValue by default during deserialization</para>
        /// </summary>
        public bool IsPriorityToDeserializeObjectAsBssomValue { get; private set; } = false;

        /// <summary>
        /// <para>序列化DateTime类型时,是否使用标准解析</para>
        /// <para>Whether standard resolution is used when serializing a DateTime type</para>
        /// </summary>
        /// <remarks>如果被序列化后的数据需要被其它语言平台解析,则此属性应该为true. This property should be true if the serialized data needs to be deserialized by another language platform</remarks>
        public bool IsUseStandardDateTime { get; private set; } = false;

        /// <summary>
        /// <para>序列化具有IDictionary行为的类型时是否将其序列化成Map1格式,如果为false,则使用Map2格式</para>
        /// <para>Whether to serialize a type with IDictionary behavior into Map1 format when serializing it,If false, use Map2 format</para>
        /// </summary>
        /// <remarks>Map1格式更适合动态键值对结构,因此推荐此属性为true. The Map1 format is more suitable for dynamic key-value pair structure, so this attribute is recommended to be true</remarks>
        public bool IDictionaryIsSerializeMap1Type { get; private set; } = true;

        /// <summary>
        /// <para>获取将<see cref="Security"/>属性设置为新值的<see cref="BssomSerializerOptions"/>副本</para>
        /// <para>Get a copy of <see cref="BssomSerializerOptions"/> with the <see cref="Security"/> attribute set to the new value</para>
        /// </summary>
        /// <param name="security"><see cref="Security"/>的新值. The new value for the <seealso cref="Security"/></param>
        /// <returns>一个新的实例,实例内的其它属性都没有改变. The new instance,none of the other properties within the instance change</returns>
        public BssomSerializerOptions WithSecurity(BssomSecurity security)
        {
            if (security is null)
            {
                throw new ArgumentNullException(nameof(security));
            }

            if (Security == security)
            {
                return this;
            }

            BssomSerializerOptions result = Clone();
            result.Security = security;
            return result;
        }

        /// <summary>
        /// <para>获取将<see cref="FormatterResolver"/>属性设置为新值的<see cref="BssomSerializerOptions"/>副本</para>
        /// <para>Get a copy of <see cref="BssomSerializerOptions"/> with the <see cref="FormatterResolver"/> attribute set to the new value</para>
        /// </summary>
        /// <param name="resolver"><see cref="FormatterResolver"/>的新值. The new value for the <seealso cref="FormatterResolver"/></param>
        /// <returns>一个新的实例,实例内的其它属性都没有改变. The new instance,none of the other properties within the instance change</returns>
        public BssomSerializerOptions WithFormatterResolver(IFormatterResolver resolver)
        {
            if (FormatterResolver == resolver)
            {
                return this;
            }

            BssomSerializerOptions result = Clone();
            result.FormatterResolver = resolver;
            return result;
        }

        /// <summary>
        ///  <para>获取将<see cref="IsPriorityToDeserializeObjectAsBssomValue"/>属性设置为新值的<see cref="BssomSerializerOptions"/>副本</para>
        /// <para>Get a copy of <see cref="BssomSerializerOptions"/> with the <see cref="IsPriorityToDeserializeObjectAsBssomValue"/> attribute set to the new value</para>
        /// </summary>
        /// <param name="isPriorityToDeserializeObjectAsBssomValue"><see cref="IsPriorityToDeserializeObjectAsBssomValue"/>的新值. The new value for the <seealso cref="IsPriorityToDeserializeObjectAsBssomValue"/></param>
        /// <returns>一个新的实例,实例内的其它属性都没有改变. The new instance,none of the other properties within the instance change</returns>
        public BssomSerializerOptions WithIsPriorityToDeserializeObjectAsBssomValue(bool isPriorityToDeserializeObjectAsBssomValue)
        {
            if (IsPriorityToDeserializeObjectAsBssomValue == isPriorityToDeserializeObjectAsBssomValue)
            {
                return this;
            }

            BssomSerializerOptions result = Clone();
            result.IsPriorityToDeserializeObjectAsBssomValue = isPriorityToDeserializeObjectAsBssomValue;
            return result;
        }

        /// <summary>
        /// <para>获取将<see cref="IsUseStandardDateTime"/>属性设置为新值的<see cref="BssomSerializerOptions"/>副本</para>
        /// <para>Get a copy of <see cref="BssomSerializerOptions"/> with the <see cref="IsUseStandardDateTime"/> attribute set to the new value</para>
        /// </summary>
        /// <param name="isUseStandardDateTime"><see cref="IsUseStandardDateTime"/>的新值. The new value for the <seealso cref="IsUseStandardDateTime"/></param>
        /// <returns>一个新的实例,实例内的其它属性都没有改变. The new instance,none of the other properties within the instance change</returns>
        public BssomSerializerOptions WithIsUseStandardDateTime(bool isUseStandardDateTime)
        {
            if (IsUseStandardDateTime == isUseStandardDateTime)
            {
                return this;
            }

            BssomSerializerOptions result = Clone();
            result.IsUseStandardDateTime = isUseStandardDateTime;
            return result;
        }

        /// <summary>
        /// <para>获取将<see cref="IDictionaryIsSerializeMap1Type"/>属性设置为新值的<see cref="BssomSerializerOptions"/>副本</para>
        /// <para>Get a copy of <see cref="BssomSerializerOptions"/> with the <see cref="IDictionaryIsSerializeMap1Type"/> attribute set to the new value</para>
        /// </summary>
        /// <param name="iDictionaryIsSerializeMap1Type"><see cref="IDictionaryIsSerializeMap1Type"/>的新值. The new value for the <seealso cref="IDictionaryIsSerializeMap1Type"/></param>
        /// <returns>一个新的实例,实例内的其它属性都没有改变. The new instance,none of the other properties within the instance change</returns>
        public BssomSerializerOptions WithIDictionaryIsSerializeMap1Type(bool iDictionaryIsSerializeMap1Type)
        {
            if (IDictionaryIsSerializeMap1Type == iDictionaryIsSerializeMap1Type)
            {
                return this;
            }

            BssomSerializerOptions result = Clone();
            result.IDictionaryIsSerializeMap1Type = iDictionaryIsSerializeMap1Type;
            return result;
        }

        protected virtual BssomSerializerOptions Clone()
        {
            if (GetType() != typeof(BssomSerializerOptions))
            {
                throw new NotSupportedException($"The derived type {GetType().FullName} did not override the {nameof(Clone)} method as required.");
            }

            return new BssomSerializerOptions(this);
        }
    }
}
