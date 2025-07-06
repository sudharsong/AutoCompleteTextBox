using codecrafterskafka.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Base
{
    internal class RequestHeader
    {
        private int messageHeaderLength;

        private byte[] buffer;

        public short ApiKey { get; }
        public short ApiVersion { get; }
        public int CorrelationId { get; }

        public string ClientId { get; }
        public byte TagBufferField { get; }

        public int MessageHeaderLength { get { return messageHeaderLength; } }

        

        public RequestHeader(byte[] buffer)
        {
            this.buffer = buffer;
            ApiKey = this.buffer.ReadInt16FromBuffer(ref messageHeaderLength); // Read the API key (2 bytes) 
            ApiVersion = this.buffer.ReadInt16FromBuffer(ref messageHeaderLength); // Read the API Version (2 bytes) 
            CorrelationId = this.buffer.ReadInt32FromBuffer(ref messageHeaderLength); // Read the Correlation (4 bytes)
            var clientIdLength = this.buffer.ReadInt16FromBuffer(ref messageHeaderLength); // Read the Correlation (4 bytes)
            ClientId = this.buffer.ReadStringFromBuffer(ref messageHeaderLength, clientIdLength); // Read the Client ID (variable length)
            TagBufferField = this.buffer.ReadByteFromBuffer(ref messageHeaderLength); // Read the tag buffer field (1 byte)
        }

    }
}
