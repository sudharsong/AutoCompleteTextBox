using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class MetaDataRecord
    {
        public byte FrameVersion { get; set; }

        public RecordType Type { get; set; }

        public byte Version { get; set; }
        public short ValueLength { get; internal set; }

        public virtual void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(FrameVersion);
            writer.WriteToBuffer((byte)Type);
            writer.WriteToBuffer(Version);
            writer.WriteToBuffer(ValueLength);
        }
    }
}
