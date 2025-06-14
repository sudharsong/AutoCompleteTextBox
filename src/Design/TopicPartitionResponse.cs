using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class TopicPartitionResponse : Response
    {
        private TopicParitionResponseHeader header;
        private TopicParitionResponseBody body;


        public TopicPartitionResponse(TopicParitionResponseHeader header, TopicParitionResponseBody body)
        {
            this.header = header;
            this.body = body;
            //this.messageSize = header.MessageHeaderLength + body.MessageBodyLength; // Calculate the total message size
        }

        public override ReadOnlyMemory<byte> GetResponse(ArrayBufferWriter<byte> writer)
        {            
            var lengthSpan = writer.GetSpan(4);
            writer.Advance(4); // reserve space for length  

            header.WriteResponse(writer);
            body.WriteResponse(writer);
            //writer.WriteInt32ToBuffer(0); //correlationId            
            //writer.WriteInt16ToBuffer(0); //ErrorCode
            //writer.WriteByteToBuffer(3); //Api key version array length

            //writer.WriteInt16ToBuffer(18); //Api key
            //writer.WriteInt16ToBuffer(0); //Api key min version 
            //writer.WriteInt16ToBuffer(4); //Api key max version
            //writer.WriteByteToBuffer(0); //Tag field 

            //writer.WriteInt16ToBuffer(75); //api valid key
            //writer.WriteInt16ToBuffer(0); //Api key min version
            //writer.WriteInt16ToBuffer(0); //Api key max version
            //writer.WriteByteToBuffer(0); //Tag field

            //writer.WriteInt32ToBuffer(120); //Throttle time
            //writer.WriteByteToBuffer(0); //Tag field

            this.MessageSize = writer.WrittenCount - 4; // calculate the length of the response   
            BinaryPrimitives.WriteInt32BigEndian(lengthSpan, MessageSize); // write the length to the reserved space 
            return writer.WrittenMemory;
        }
    }
}
