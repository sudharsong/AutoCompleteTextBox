using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Base
{
    internal abstract class Response
    {
        public ResponseHeader Header { get; protected set; }

        public ResponseBody Body { get; protected set; }

        public int MessageSize { get; protected set; }

        public virtual ReadOnlyMemory<byte> GetResponse(ArrayBufferWriter<byte> writer)
        {
            var lengthSpan = writer.GetSpan(4);
            writer.Advance(4); // reserve space for length  

            Header.WriteResponse(writer);
            Body.WriteResponse(writer);

            MessageSize = writer.WrittenCount - 4; // calculate the length of the response   
            BinaryPrimitives.WriteInt32BigEndian(lengthSpan, MessageSize); // write the length to the reserved space 
            return writer.WrittenMemory;
        }
    }
}



