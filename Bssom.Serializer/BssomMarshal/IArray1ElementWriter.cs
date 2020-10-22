using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bssom.Serializer.Binary;
using Bssom.Serializer.BssMap.KeyResolvers;
using Bssom.Serializer.Internal;
using Bssom.Serializer.BssomBuffer;
using Bssom.Serializer.BssMap;
using Bssom.Serializer.Resolver;
namespace Bssom.Serializer.Internal
{
    internal interface IArray1ElementWriter
    {
        void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value);
        object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo);
    }

    internal interface IArray1ElementWriter<T> : IArray1ElementWriter
    {
        void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, T value);
        T ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo);
    }


    internal sealed class ObjectArray1ElementWriter : IArray1ElementWriter<Object>, IArray1ElementWriter<BssomValue>, IArray1ElementWriter<BssomChar>, IArray1ElementWriter<BssomBoolean>, IArray1ElementWriter<BssomDateTime>, IArray1ElementWriter<BssomDecimal>, IArray1ElementWriter<BssomFloat>, IArray1ElementWriter<BssomGuid>, IArray1ElementWriter<BssomNumber>
    {
        public readonly static ObjectArray1ElementWriter Instance = new ObjectArray1ElementWriter();

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, Object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            var array1ElementWriter = Array1ElementWriterContainers.GetArray1ElementWriter(offsetInfo.Array1ElementTypeIsNativeType, offsetInfo.Array1ElementType);
            array1ElementWriter.WriteObjectElement(ref writer, option, offsetInfo, value);
        }
        public Object ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            var array1ElementWriter = Array1ElementWriterContainers.GetArray1ElementWriter(offsetInfo.Array1ElementTypeIsNativeType, offsetInfo.Array1ElementType);
            return array1ElementWriter.ReadObjectElement(ref reader, option, offsetInfo);
        }

        public void WriteObjectElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, object value)
        {
            throw new NotImplementedException();
        }

        public object ReadObjectElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            throw new NotImplementedException();
        }

        #region BssomValue

        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomValue value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomValue IArray1ElementWriter<BssomValue>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomChar value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomChar IArray1ElementWriter<BssomChar>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomChar)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomBoolean value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomBoolean IArray1ElementWriter<BssomBoolean>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomBoolean)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomDateTime value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomDateTime IArray1ElementWriter<BssomDateTime>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomDateTime)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomDecimal value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomDecimal IArray1ElementWriter<BssomDecimal>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomDecimal)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomFloat value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomFloat IArray1ElementWriter<BssomFloat>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomFloat)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomGuid value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomGuid IArray1ElementWriter<BssomGuid>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomGuid)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        public void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, BssomNumber value)
        {
            this.WriteElement(ref writer, option, offsetInfo, value.RawValue);
        }
        BssomNumber IArray1ElementWriter<BssomNumber>.ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return (BssomNumber)BssomValue.Create(this.ReadElement(ref reader, option, offsetInfo));
        }
        #endregion
    }
}