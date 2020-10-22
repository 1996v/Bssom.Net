using System.Threading;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>反序列化期间使用的上下文</para>
    /// <para>The context used during deserialization</para>
    /// </summary>
    public struct BssomDeserializeContext
    {
        /// <summary>
        /// <para>反序列化期间使用的配置</para>
        /// <para>The configuration used during deserialization</para>
        /// </summary>
        public BssomSerializerOptions Option { get; set; }

        /// <summary>
        /// <para>在反序列化期间可用于存储和读取的数据容器</para>
        /// <para>A data container that can be used to store and read during the fetch of the deserialization</para>
        /// </summary>
        public ContextDataSlots ContextDataSlots { get; set; }

        /// <summary>
        /// <para>此反序列化操作的取消标记</para>
        /// <para>The cancellation token for this deserialization operation</para>
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// <para>当前反序列化时的深度</para>
        /// <para>Depth of current deserialization</para>
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomDeserializeContext"/> struct.
        /// </summary>
        public BssomDeserializeContext(BssomSerializerOptions option) : this(option, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomDeserializeContext"/> struct.
        /// </summary>
        public BssomDeserializeContext(BssomSerializerOptions option, CancellationToken canceToken)
        {
            this.Option = option;
            this.ContextDataSlots = default;
            this.CancellationToken = canceToken;
            this.Depth = 0;
        }
    }
}