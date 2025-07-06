using codecrafterskafka.src;
using src.Design.Base;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.TopicPartition
{
    internal class TopicParitionResponseBodyV0 : ResponseBody
    {
        public int ThrottleTime { get; }

        public byte Cursor
        {
            get; set;
        }

        public ResponseTopic[]? Topics
        {
            get; set;
        }

        public byte TopicsLength
        {
            get
            {
                return (byte)(Topics.Length + 1);
            }
        }


        public byte TagBuffer
        {
            get; set;
        }

        public override void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            Cursor = 0xff;
            writer.WriteToBuffer(ThrottleTime);
            writer.WriteToBuffer(TopicsLength); // Write the length of the topics array (1 byte)
            if (Topics != null)
            {
                foreach (var topic in Topics)
                {
                    topic.WriteResponse(writer); // Write each topic's response
                }
            }

            writer.WriteToBuffer(Cursor); // Write the cursor (1 byte, nullable)
            writer.WriteToBuffer(TagBuffer);
        }
    }
}
