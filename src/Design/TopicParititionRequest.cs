using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class TopicParititionRequest
    {
        private byte[] buffer;

        private int offset;
        private TopicPartiotionRequestHeader header;
        private TopicPartitionRequestBody body;

        public TopicParititionRequest(byte[] buffer) {
            this.buffer = buffer;
            this.header = new TopicPartiotionRequestHeader(this.buffer);
            this.offset = this.header.MessageHeaderLength;
            this.body = new TopicPartitionRequestBody(this.buffer, this.offset); // Read the body of the request
        }

        public TopicPartiotionRequestHeader Head { get
            {
                return this.header;
            }
        }

        public TopicPartitionRequestBody Body
        {
            get
            {
                return this.body;
            }
        }
    }
}
