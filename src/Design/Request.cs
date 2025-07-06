using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class Request
    {
        private byte[] buffer;

        private int offset;
        private RequestHeader header;
        private RequestBody body;

        public Request(byte[] buffer) {
            this.buffer = buffer;
            this.header = new RequestHeader(this.buffer);
            this.offset = this.header.MessageHeaderLength;
            if(header.ApiKey == 18 && header.ApiVersion >= 0 && header.ApiVersion <= 4)
            {
                this.body = new ApiVersionRequestBodyV4();
            }
            else if (header.ApiKey == 75 && header.ApiVersion >= 0 && header.ApiVersion <= 0)
            {
                this.body = new TopicPartitionRequestBodyV0();
            }
            else if (header.ApiKey == 1 && header.ApiVersion >= 0 && header.ApiVersion <= 16)
            {
                this.body = new FetchRequestBodyV16();
            }

            this.body?.PopulateBody(this.buffer, this.offset);
        }

        public RequestHeader Head { get
            {
                return this.header;
            }
        }

        public RequestBody Body
        {
            get
            {
                return this.body;
            }
        }
    }
}
