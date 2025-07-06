using codecrafterskafka.src;
using src.Design.Base;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.TopicPartition
{
    internal class TopicParitionResponseHeaderV0 : ResponseHeader
    {
        public TopicParitionResponseHeaderV0(int correlationId) : base(correlationId)
        {
            TagBuffer   = 0;
        }
        public byte TagBuffer { get; }

        public override void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(CorrelationId);
            writer.WriteToBuffer(TagBuffer);
        }
    }
}
