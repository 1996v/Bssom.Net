using System.Collections.Generic;
using System.Threading;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>获取序列化大小期间使用的上下文</para>
    /// <para>Context used during get serialization size</para>
    /// </summary>
    public struct BssomSizeContext
    {
        /// <summary>
        /// <para>在获取序列化大小期间使用的配置</para>
        /// <para>Configuration used during get serialization size</para>
        /// </summary>
        public BssomSerializerOptions Option { get; set; }

        /// <summary>
        /// <para>在获取序列化大小期间可用于存储和读取的数据容器</para>
        /// <para>A data container that can be used to store and read during the fetch of the serialization size</para>
        /// </summary>
        public ContextDataSlots ContextDataSlots { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSizeContext"/> struct.
        /// </summary>
        public BssomSizeContext(BssomSerializerOptions option)
        {
            this.Option = option;
            this.ContextDataSlots = default;
        }
    }
}