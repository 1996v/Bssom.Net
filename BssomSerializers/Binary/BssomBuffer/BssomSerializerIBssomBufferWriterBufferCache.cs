using System;
using System.Runtime.CompilerServices;

namespace BssomSerializers.BssomBuffer
{
    /// <summary>
    /// <para>提供<see cref="BssomSerializer"/>默认的<see cref="IBssomBufferWriter"/>所使用的内部缓存,公开缓存是不安全的,仅限于上层程序在序列化期间外复用该静态缓存</para>
    /// <para>Provide the internal cache used by <see cref="BssomSerializer"/>default <see cref="IBssomBufferWriter"/>. Exposing the cache is not secure, this is limited to upper-level programs that reuse the static cache not during serialization</para>
    /// </summary>
    public static class BssomSerializerIBssomBufferWriterBufferCache
    {
        /// <summary>
        /// <para><see cref="GetUnsafeBssomArrayCache"/>获得的缓存大小</para>
        /// <para>cache size obtained by <see cref="GetUnsafeBssomArrayCache"/></para>
        /// </summary>
        public const int BssomArrayCacheSize = ushort.MaxValue;

        [ThreadStatic]
        private static byte[] internalCache;

        /// <summary>
        /// <para>获取<see cref="BssomSerializer"/>在序列化期间使用的缓存,该缓存禁止修改,且不能在序列化期间使用,否则会出现无法预测的错误</para>
        /// <para>Get the cache used by <see cref="BssomSerializer"/> during serialization. The cache must not be modified and cannot be used during serialization, otherwise unpredictable errors will occur</para>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GetUnsafeBssomArrayCache()
        {
            if (internalCache == null)
                internalCache = new byte[BssomArrayCacheSize];
            return internalCache;
        }
    }
}