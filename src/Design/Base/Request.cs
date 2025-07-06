using src.Design.ApiVersion;
using src.Design.Fetch;
using src.Design.TopicPartition;

namespace src.Design.Base
{
    internal class Request
    {
        private byte[] buffer;

        private int offset;
        private RequestHeader header;
        private RequestBody body;

        public Request(byte[] buffer) {
            this.buffer = buffer;
            header = new RequestHeader(this.buffer);
            offset = header.MessageHeaderLength;
            if(header.ApiKey == 18 && header.ApiVersion >= 0 && header.ApiVersion <= 4)
            {
                body = new ApiVersionRequestBodyV4();
            }
            else if (header.ApiKey == 75 && header.ApiVersion >= 0 && header.ApiVersion <= 0)
            {
                body = new TopicPartitionRequestBodyV0();
            }
            else if (header.ApiKey == 1 && header.ApiVersion >= 0 && header.ApiVersion <= 16)
            {
                body = new FetchRequestBodyV16();
            }

            body?.PopulateBody(this.buffer, offset);
        }

        public RequestHeader Head { get
            {
                return header;
            }
        }

        public RequestBody Body
        {
            get
            {
                return body;
            }
        }
    }
}
