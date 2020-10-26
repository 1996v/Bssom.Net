//using System.Runtime.CompilerServices;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>IBssomBuffer的类型写入器</para>
    /// <para>A types writer for the IBssomBuffer.</para>
    /// </summary>
    public interface IBssomBufferWriter
    {
        /// <summary>
        /// <para>写入器的当前位置</para>
        /// <para>The current position within the writer.</para>
        /// </summary>
        long Position { get; }

        /// <summary>
        /// <para>设置当前写入器的位置</para>
        /// <para>Sets the position within the current writer.</para>
        /// </summary>
        /// <param name="position">相对于<paramref name="orgin"/>参数的字节偏移量 A byte offset relative to the <paramref name="orgin"/> parameter</param>
        /// <param name="orgin">指示用于获得新位置的参考点 indicating the reference point used to obtain the new position.</param>
        /// <remarks>若position超出了写入器所推进过的范围,则需要产生异常 If position is beyond the range advanced by the writer, an exception needs to be generated</remarks>
        void Seek(long position, BssomSeekOrgin orgin = BssomSeekOrgin.Begin);

        /// <summary>
        /// <para>设置当前写入器的位置,并且不对<paramref name="Buffered"/>的边界进行验证.</para>
        /// <para>Set the position of the current writer, and do not verify the boundary of <paramref name="Buffered"/>.</para>
        /// </summary>
        /// <param name="position">相对于<paramref name="orgin"/>的字节偏移量 A byte offset relative to the <paramref name="orgin"/> parameter</param>
        /// <param name="orgin">指示用于获得新位置的参考点 indicating the reference point used to obtain the new position.</param>
        void SeekWithOutVerify(long position, BssomSeekOrgin orgin);

        /// <summary>
        /// <para>从当前位置获取用于写入的字节序列的引用</para>
        /// <para>Get a reference to the byte sequence for writing from the current buffer</para>
        /// </summary>
        /// <param name="sizeHint">需要写入字节的大小 The size of bytes to be written</param>
        /// <returns>第一个字节序列的引用 A reference to the first byte sequence</returns>
        ref byte GetRef(int sizeHint);

        /// <summary>
        /// <para>在<see cref="BssomFieldMarshaller.TryWrite"/>中,当前位置是否能提供指定大小的字节序列引用以用来提供内部某些类型写入的性能</para>
        /// <para>In <see cref="BssomFieldMarshaller.TryWrite"/>,Whether the current position can provide a byte sequence reference of specified size to be used to provide performance for some types of internal writes</para>
        /// </summary>
        /// <param name="size">需要写入字节的大小 The size of bytes to be written</param>
        /// <returns>在<see cref="BssomFieldMarshaller.TryWrite"/>某些类型中,如果可以提供,则可以更快的写入元素,否则以默认逻辑来写入元素. In <see cref="BssomFieldMarshaller.TryWrite"/> some types, if you can provide, can quickly write elements, or otherwise default logic to write elements</returns>
        bool CanGetSizeRefForProvidePerformanceInTryWrite(int size);

        /// <summary>
        /// <para>在缓冲区中已写入字节的数目</para>
        /// <para>The number of bytes written in the buffer</para>
        /// </summary>
        long Buffered { get; }

        /// <summary>
        /// <para>用于指示已写入缓冲区的部分</para>
        /// <para>Used to indicate that part of the buffer has been written to.</para>
        /// </summary>
        void Advance(int count);

        /// <summary>
        /// <para>获取当前写入器所使用的缓冲区</para>
        /// <para>Gets the buffer used by the current writer</para>
        /// </summary>
        IBssomBuffer GetBssomBuffer();
    }
}
