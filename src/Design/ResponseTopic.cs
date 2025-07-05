using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class ResponseTopic
    {
        private Guid uuid;

        public short ErrorCode { get; set;}

        public byte ContentLength
        {
            get
            {
                return (byte)(UTF8Content.Length+1);
            }
        }

        public string? Content
        {
            get;
            set;
        }


        public Guid UUID
        {
            get;
            set;
        }

        public byte[] UTF8Content
        {
            get
            {
                if (string.IsNullOrEmpty(Content))
                {
                    return Array.Empty<byte>();
                }

                return Encoding.UTF8.GetBytes(Content);
            }
        }


        public bool IsInternal
        {
            get; set;
        }

        public byte PartitionsCount
        {
            get; set;
        }

        public Partition[]? Partitions
        {
            get; set;
        }   

        public int Operations
        {
            get; set;
        }

        public byte TagBuffer
        {
            get; set;
        }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(this.ErrorCode);
            writer.WriteToBuffer(this.ContentLength); // Write the length of the content (1 byte)
            writer.WriteToBuffer(UTF8Content); // Write the UTF8 content
            writer.WriteToBuffer(UUID.ToByteArray(true)); // Write the UUID as bytes
            writer.WriteToBuffer(this.IsInternal ? (byte)1 : (byte)0); // Write the internal flag (1 byte)
            writer.WriteToBuffer((byte)(this.PartitionsCount+1)); // Write the partitions count (1 byte)
            if(this.PartitionsCount > 0)
            {
                
                for (int i = 0; i < Partitions.Length; i++)
                {
                    Partition? partition = Partitions[i];
                    partition.WriteResponse(writer); // Write each partition's response
                }
            }

            writer.WriteToBuffer(this.Operations); // Write the operations (4 bytes)
            writer.WriteToBuffer(this.TagBuffer); // Write the tag buffer (1 byte)}
        }
    }
}
