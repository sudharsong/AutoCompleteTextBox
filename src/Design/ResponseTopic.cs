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
                return (byte)(Encoding.UTF8.GetBytes(Content).Length + 1);
            }
        }

        public string Content
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
            writer.WriteInt16ToBuffer(this.ErrorCode);
            writer.WriteByteToBuffer(this.ContentLength); // Write the length of the content (1 byte)
            writer.WriteBytesToBuffer(UTF8Content); // Write the UTF8 content
            writer.WriteBytesToBuffer(UUID.ToByteArray(true)); // Write the UUID as bytes
            writer.WriteByteToBuffer(this.IsInternal ? (byte)1 : (byte)0); // Write the internal flag (1 byte)
            writer.WriteByteToBuffer(this.PartitionsCount); // Write the partitions count (1 byte)
            writer.WriteInt32ToBuffer(this.Operations); // Write the operations (4 bytes)
            writer.WriteByteToBuffer(this.TagBuffer); // Write the tag buffer (1 byte)}
        }
    }
}
