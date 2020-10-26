using System.Threading;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>序列化期间使用的上下文</para>
    /// <para>The context used during serialization</para>
    /// </summary>
    public struct BssomSerializeContext
    {
        private ContextDataSlots _contextDataSlots;

        /// <summary>
        /// <para>序列化期间使用的配置</para>
        /// <para>The configuration used during serialization</para>
        /// </summary>
        public BssomSerializerOptions Option { get; set; }

        /// <summary>
        /// <para>此序列化操作的取消标记</para>
        /// <para>The cancellation token for this serialization operation</para>
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// <para>在序列化期间可用于存储和读取的数据容器</para>
        /// <para>A data container that can be used to store and read during the fetch of the serialization</para>
        /// </summary>
        public ContextDataSlots ContextDataSlots
        {
            get
            {
                if (_contextDataSlots == null)
                {
                    _contextDataSlots = new ContextDataSlots();
                }

                return _contextDataSlots;
            }
            set => _contextDataSlots = value;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializeContext"/> struct.
        /// </summary>
        public BssomSerializeContext(BssomSerializerOptions option) : this(option, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSerializeContext"/> struct.
        /// </summary>
        public BssomSerializeContext(BssomSerializerOptions option, CancellationToken canceToken)
        {
            _contextDataSlots = default;
            Option = option;
            CancellationToken = canceToken;
        }
    }
}