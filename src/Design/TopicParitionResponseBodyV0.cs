using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class TopicParitionResponseBodyV0
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
                return (byte)(this.Topics.Length + 1);
            }
        }

        //public Partition[] Partitions
        //{
        //    get; set;
        //}

        //public byte PartitionsLength
        //{
        //    get
        //    {
        //        return (byte)((this.Partitions?.Length ?? 0) + 1);
        //    }
        //}


        public byte TagBuffer
        {
            get; set;
        }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            this.Cursor = 0xff;
            writer.WriteToBuffer(this.ThrottleTime);
            Console.WriteLine($"Topic Length : {this.TopicsLength}");
            writer.WriteToBuffer(this.TopicsLength); // Write the length of the topics array (1 byte)
            if (this.Topics != null)
            {
                foreach (var topic in this.Topics)
                {
                    topic.WriteResponse(writer); // Write each topic's response
                }
            }

            //writer.WriteToBuffer(this.PartitionsLength); // Write the length of the topics array (1 byte)
            //if (this.Partitions != null)
            //{
            //    foreach (var partition in this.Partitions)
            //    {
            //        partition.WriteResponse(writer); // Write each topic's response
            //    }
            //}

            writer.WriteToBuffer(this.Cursor); // Write the cursor (1 byte, nullable)
            writer.WriteToBuffer(this.TagBuffer);
        }
    }
}
