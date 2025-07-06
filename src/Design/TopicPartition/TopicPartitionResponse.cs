using src.Design.Base;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.TopicPartition
{
    internal class TopicPartitionResponse : Response
    {
        public TopicPartitionResponse(TopicParitionResponseHeaderV0 header, TopicParitionResponseBodyV0 body)
        {
            Header = header;
            Body = body;
            //this.messageSize = header.MessageHeaderLength + body.MessageBodyLength; // Calculate the total message size
        }

        //public override ReadOnlyMemory<byte> GetResponse(ArrayBufferWriter<byte> writer)
        //{            
        //    var lengthSpan = writer.GetSpan(4);
        //    writer.Advance(4); // reserve space for length  

        //    Header.WriteResponse(writer);
        //    Body.WriteResponse(writer);

        //    this.MessageSize = writer.WrittenCount - 4; // calculate the length of the response   
        //    BinaryPrimitives.WriteInt32BigEndian(lengthSpan, MessageSize); // write the length to the reserved space 
        //    return writer.WrittenMemory;
        //}
    }
}
