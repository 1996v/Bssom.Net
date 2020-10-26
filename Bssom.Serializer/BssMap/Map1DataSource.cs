using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bssom.Serializer.BssMap
{
    internal class Map1DataSource<TKey, TValue> : IMapDataSource<TKey, TValue>
    {
        internal BssomReader _reader;
        internal BssomDeserializeContext _context;

        public Map1DataSource(BssomReader reader, BssomDeserializeContext context, bool isOnlyReadFieldOffset = false)
        {
            _reader = reader;
            _context = context;
            KeyFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TKey>();
            if (!isOnlyReadFieldOffset)
            {
                ValueFormatter = context.Option.FormatterResolver.GetFormatterWithVerify<TValue>();
            }

            ReadPosition = reader.Position;
            int size = reader.ReadVariableNumber();
            EndPosition = reader.Position + size;
            Count = reader.ReadVariableNumber();
        }

        public BssomReader Reader => _reader;
        public BssomDeserializeContext Context => _context;
        public int Count { get; }
        public long ReadPosition { get; }
        public long EndPosition { get; }
        public IBssomFormatter<TKey> KeyFormatter { get; }
        public IBssomFormatter<TValue> ValueFormatter { get; }
        public bool IsOnlyReadFieldOffset => ValueFormatter == null;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                TKey key = KeyFormatter.Deserialize(ref _reader, ref _context);
                if (IsOnlyReadFieldOffset)
                {
                    BssomFieldOffsetInfo offset = new BssomFieldOffsetInfo(_reader.Position);
                    yield return new KeyValuePair<TKey, TValue>(key, Unsafe.As<BssomFieldOffsetInfo, TValue>(ref offset));
                }
                else
                {
                    yield return new KeyValuePair<TKey, TValue>(key, ValueFormatter.Deserialize(ref _reader, ref _context));
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}