//using System.Runtime.CompilerServices;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>Bssom字段在缓冲区中的位置信息</para>
    /// <para>Location information of the Bssom field in the buffer</para>
    /// </summary>
    public struct BssomFieldOffsetInfo
    {
        /// <summary>
        /// <para>Offset为0的默认实例</para>
        /// <para>Default instance with Offset of 0</para>
        /// </summary>
        public static readonly BssomFieldOffsetInfo Zero = new BssomFieldOffsetInfo(0);

        internal bool IsArray1Type;
        internal byte Array1ElementType;
        internal bool Array1ElementTypeIsNativeType;

        /// <summary>
        /// <para>通过<paramref name="offset"/>来初始化<see cref="BssomFieldOffsetInfo"/></para>
        /// <para>Use <paramref name="offset"/> to initialize <see cref="BssomFieldOffsetInfo"/></para> 
        /// </summary>
        /// <param name="offset"></param>
        public BssomFieldOffsetInfo(long offset)
        {
            IsArray1Type = false;
            Array1ElementType = default;
            Array1ElementTypeIsNativeType = false;
            Offset = offset;
        }

        /// <summary>
        /// <para>在缓冲区中的偏移量</para>
        /// <para>The offset in the buffer</para>
        /// </summary>
        public long Offset { get; internal set; }

        /// <summary>
        /// <para>该字段是否在缓冲区中存在</para>
        /// <para>Whether the field exists in the buffer</para>
        /// </summary>
        public bool IsExists => Offset != -1;
    }

}
