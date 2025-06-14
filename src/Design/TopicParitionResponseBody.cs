using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class TopicParitionResponseBody
    {
        public int ThrottleTime { get; }

        public byte Cursor
        {
            get; set;
        }

        public ResponseTopic[] Topics
        {
            get; set;
        }

        public byte TopicsLength
        {
            get
            {
                return (byte)((this.Topics?.Length ?? 0) + 1);
            }
        }   


        public byte TagBuffer
        {
            get; set;
        }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            this.Cursor = 0xff;
            writer.WriteInt32ToBuffer(this.ThrottleTime);
            writer.WriteByteToBuffer(this.TopicsLength); // Write the length of the topics array (1 byte)
            if (this.Topics != null)
            {
                foreach (var topic in this.Topics)
                {
                    topic.WriteResponse(writer); // Write each topic's response
                }
            }
            writer.WriteByteToBuffer(this.Cursor); // Write the cursor (1 byte, nullable)
            writer.WriteByteToBuffer(this.TagBuffer);
        }
    }
}
