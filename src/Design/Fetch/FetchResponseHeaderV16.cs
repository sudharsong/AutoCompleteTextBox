using codecrafterskafka.src;
using src.Design.Base;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Fetch
{
    internal class FetchResponseHeaderV16 : ResponseHeader
    {
        public byte TagBuffer { get; }
        public FetchResponseHeaderV16(int correlationId) : base(correlationId)
        {
        }

        public override void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(CorrelationId);
            writer.WriteToBuffer(TagBuffer);
        }
    }
}
