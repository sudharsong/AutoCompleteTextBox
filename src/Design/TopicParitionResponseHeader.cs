using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class TopicParitionResponseHeader : Header
    {
        public TopicParitionResponseHeader(int correlationId) : base(correlationId)
        {
            this.TagBuffer   = 0;
        }
        public byte TagBuffer { get; }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteInt32ToBuffer(this.CorrelationId);
            writer.WriteByteToBuffer(this.TagBuffer);
        }
    }
}
