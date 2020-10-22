using System;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>与安全性相关的设置,目前仅在反序列化期间提供对数据深度的限制</para>
    /// <para>Settings related to security, currently only provides a limit on the depth of data during deserialization</para>
    /// </summary>
    public class BssomSecurity
    {
        /// <summary>
        /// <para>获取一个实例,该实例预先配置了可以忽略所有保护措施的设置,用于反序列化完全信任的Bssom数据</para>
        /// <para>Get an instance that is pre-configured to ignore all protection settings. It is used to deserialize fully trusted  Bssom data.</para>
        /// </summary>
        public static readonly BssomSecurity TrustedData = new BssomSecurity();

        /// <summary>
        /// <para>获取一个实例,该实例预先配置了合理的保护措施的设置,用于反序列化不可信的Bssom数据</para>
        /// <para>Gets an instance preconfigured with protections applied with reasonable settings for deserializing untrusted Bssom data.</para>
        /// </summary>

        public static readonly BssomSecurity UntrustedData = new BssomSecurity
        {
            MaximumObjectGraphDepth = 500,
        };

        private BssomSecurity() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BssomSecurity"/> class.
        /// </summary>
        protected BssomSecurity(BssomSecurity copyFrom)
        {
            if (copyFrom is null)
            {
                throw new ArgumentNullException(nameof(copyFrom));
            }

            this.MaximumObjectGraphDepth = copyFrom.MaximumObjectGraphDepth;
        }

        /// <summary>
        /// <para>获取可反序列化的对象的最大深度</para>
        /// <para>Gets the maximum depth of an object graph that may be deserialized.</para>
        /// </summary>
        public int MaximumObjectGraphDepth { get; private set; } = int.MaxValue;

        /// <summary>
        /// <para>获取这些选项的副本,并将<see cref="MaximumObjectGraphDepth"/>属性设置为新值</para>
        /// <para>Gets a copy of these options with the <see cref="MaximumObjectGraphDepth"/> property set to a new value.</para>
        /// </summary>
        /// <param name="maximumObjectGraphDepth"></param>
        /// <returns></returns>
        public BssomSecurity WithMaximumObjectGraphDepth(int maximumObjectGraphDepth)
        {
            if (this.MaximumObjectGraphDepth == maximumObjectGraphDepth)
            {
                return this;
            }

            var clone = this.Clone();
            clone.MaximumObjectGraphDepth = maximumObjectGraphDepth;
            return clone;
        }

        /// <summary>
        /// <para>检查反序列化的深度并将其递增1</para>
        /// <para>Checks the depth of the deserializing graph and increments it by 1.</para>
        /// </summary>
        /// <param name="context"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DepthStep(ref BssomDeserializeContext context)
        {
            if (context.Depth >= MaximumObjectGraphDepth)
                ThrowInsufficientExecutionStackException();

            context.Depth++;
        }

        protected virtual BssomSecurity Clone() => new BssomSecurity(this);

        private void ThrowInsufficientExecutionStackException()
        {
            throw new InsufficientExecutionStackException($"This msgpack sequence has an object graph that exceeds the maximum depth allowed of .");
        }
    }
}
