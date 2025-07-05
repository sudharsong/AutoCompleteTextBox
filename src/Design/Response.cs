using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal abstract class Response
    {
        public int MessageSize { get; protected set; }

        public abstract ReadOnlyMemory<byte> GetResponse(ArrayBufferWriter<byte> writer);
    }
}
