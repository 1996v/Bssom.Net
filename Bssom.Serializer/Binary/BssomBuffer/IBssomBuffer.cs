//using System.Runtime.CompilerServices;

namespace Bssom.Serializer
{
    /// <summary>
    /// <para>一个用于BssomSerializer的缓冲区</para>
    /// <para>A buffer for BssomSerializer format</para>
    /// </summary>
    public interface IBssomBuffer
    {
        /// <summary>
        /// <para>缓冲区中的当前位置</para>
        /// <para>The current position within the buffer.</para>
        /// </summary>
        long Position { get; }

        /// <summary>
        /// <para>从当前缓冲区中的位置读取指定大小序列的引用</para>
        /// <para>Read the reference of the specified size byte sequence from the position in the current buffer</para>
        /// </summary>
        /// <param name="size">从当前缓冲区中读取的字节大小 The size of bytes read from the current buffer</param>
        /// <returns>第一个字节序列的引用 A reference to the first byte sequence</returns>
        /// <remarks>若当前缓冲区中的位置所读取的长度超出缓冲区边界,则需要产生异常 If the current position in the buffer reads size than the buffer boundary, an exception needs to be raised</remarks>
        ref byte ReadRef(int size);

        /// <summary>
        /// <para>设置当前缓冲区的位置</para>
        /// <para>Sets the position within the current buffer.</para>
        /// </summary>
        /// <param name="position">相对于<paramref name="orgin"/>参数的字节偏移量 A byte offset relative to the <paramref name="orgin"/> parameter</param>
        /// <param name="orgin">指示用于获得新位置的参考点 indicating the reference point used to obtain the new position.</param>
        /// <remarks>若position超出了缓冲区的范围,则需要产生异常 If the position exceeds the range of the buffer, an exception needs to be generated</remarks>
        void Seek(long position, BssomSeekOrgin orgin = BssomSeekOrgin.Begin);

        /// <summary>
        /// <para>设置当前缓冲区的位置,并且不对<paramref name="position"/>的边界进行验证.</para>
        /// <para>Set the position of the current buffer, and do not verify the boundary of <paramref name="position"/>.</para>
        /// </summary>
        /// <param name="position">相对于<paramref name="orgin"/>的字节偏移量 A byte offset relative to the <paramref name="orgin"/> parameter</param>
        /// <param name="orgin">指示用于获得新位置的参考点 indicating the reference point used to obtain the new position.</param>
        void SeekWithOutVerify(long position, BssomSeekOrgin orgin);

        /// <summary>
        /// <para>尝试从当前缓冲区中的位置读取一个可以固定的字节序列的引用,当进行Seek操作的时候不会影响被固定字节的引用位置</para>
        /// <para>Attempt to return a fixed reference to a byte sequence from the current position in the buffer,When the Seek operation is performed, the reference position of the fixed byteRef will not be affected</para>
        /// </summary>
        /// <param name="size">从当前缓冲区中读取的字节大小 The size of bytes read from the current buffer</param>
        /// <param name="haveEnoughSizeAndCanBeFixed">如果当前位置的缓冲区不能满足指定大小的字节序列读取或者不能返回固定的字节序列的引用,则返回false,否则返回true. If the buffer at the current position cannot satisfy the byte sequence reading of the specified size or cannot return a fixed byte sequence reference, return false, otherwise return true.</param>
        /// <returns>第一个字节序列的引用 A reference to the first byte sequence</returns>
        ref byte TryReadFixedRef(int size, out bool haveEnoughSizeAndCanBeFixed);

        /// <summary>
        /// <para>用于取消由<see cref="TryReadFixedRef"/>所固定的引用,此方法的调用始终和<see  cref="TryReadFixedRef"/>对称</para>
        /// <para>Used to cancel the reference fixed by <see cref="TryReadFixedRef"/>, the call of this method is always symmetrical with <see cref="TryReadFixedRef"/></para>
        /// </summary>
        void UnFixed();
    }
}
