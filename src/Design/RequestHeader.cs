using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
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

        public int MessageHeaderLength { get { return this.messageHeaderLength; } }

        

        public RequestHeader(byte[] buffer)
        {
            this.buffer = buffer;
            this.ApiKey = this.buffer.ReadInt16FromBuffer(ref messageHeaderLength); // Read the API key (2 bytes) 
            this.ApiVersion = this.buffer.ReadInt16FromBuffer(ref messageHeaderLength); // Read the API Version (2 bytes) 
            this.CorrelationId = this.buffer.ReadInt32FromBuffer(ref messageHeaderLength); // Read the Correlation (4 bytes)
            var clientIdLength = this.buffer.ReadInt16FromBuffer(ref messageHeaderLength); // Read the Correlation (4 bytes)
            this.ClientId = this.buffer.ReadStringFromBuffer(ref messageHeaderLength, clientIdLength); // Read the Client ID (variable length)
            this.TagBufferField = this.buffer.ReadByteFromBuffer(ref messageHeaderLength); // Read the tag buffer field (1 byte)
        }

    }
}
