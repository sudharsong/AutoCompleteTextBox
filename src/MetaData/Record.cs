using System.Buffers;
using System.Text;

namespace codecrafterskafka.src.MetaData
{
    internal class Record
    {
        public int Length { get; set; }
        public byte Attributes { get; set; }
        public int TimestampDelta { get; set; }
        public int OffsetDelta { get; set; }
        public string? Key { get; set; }

        public int ValueLength { get; set; }

        public MetaDataRecord? Value { get; set; }

        public uint HeaderArrayCount { get; set; }
        public int KeyLength { get; internal set; }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteVarIntToBuffer(Length);
            writer.WriteToBuffer(Attributes);
            writer.WriteVarIntToBuffer(TimestampDelta);
            writer.WriteVarIntToBuffer(OffsetDelta);
            writer.WriteVarIntToBuffer(KeyLength);
            writer.WriteToBuffer(Encoding.UTF8.GetBytes(Key ?? string.Empty));
            writer.WriteVarIntToBuffer(ValueLength);
            if (Value != null)
            {
                Value.WriteResponse(writer);
            }
            writer.WriteVarIntToBuffer((int)HeaderArrayCount);
        }
    }
}
