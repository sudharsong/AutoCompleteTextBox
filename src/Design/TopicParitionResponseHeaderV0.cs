using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class TopicParitionResponseHeaderV0 : Header
    {
        public TopicParitionResponseHeaderV0(int correlationId) : base(correlationId)
        {
            this.TagBuffer   = 0;
        }
        public byte TagBuffer { get; }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(this.CorrelationId);
            writer.WriteToBuffer(this.TagBuffer);
        }
    }
}
