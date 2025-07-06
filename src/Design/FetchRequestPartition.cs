using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.Design
{
    internal class FetchRequestPartition
    {
        public int PartitionIndex { get; set; }

        public long FetchOffset { get; set; }

        public int MaxBytes { get; set; }

        public int CurrentLeaderEpoch { get; set; }

        public int LastFetchedEpoch { get; set; }

        public long LogStartOffset { get; set; }

        public byte TagBuffer { get; set; }

        public void PopulateBody(byte[] buffer, int offset)
        {
            this.PartitionIndex = buffer.ReadInt32FromBuffer(ref offset);
            this.CurrentLeaderEpoch = buffer.ReadInt32FromBuffer(ref offset);
            this.FetchOffset = buffer.ReadInt64FromBuffer(ref offset);
            this.LastFetchedEpoch = buffer.ReadInt32FromBuffer(ref offset);
            this.LogStartOffset = buffer.ReadInt64FromBuffer(ref offset);
            this.MaxBytes = buffer.ReadInt32FromBuffer(ref offset);
            this.TagBuffer = buffer.ReadByteFromBuffer(ref offset);
        }
    }

    internal class ForgottenTopicPartitions
    {
        public Guid TopicID { get; set; }
        public List<int> Partitions { get; set; } = new List<int>();
        public byte TagBuffer { get; set; }
        public void PopulateBody(byte[] buffer, int offset)
        {
            this.TopicID = buffer.ReadGuidFromBuffer(ref offset);
            uint numberOfPartitions = buffer.ReadUVarInt(ref offset);
            for (int i = 0; i < numberOfPartitions - 1; i++)
            {
                var partitionIndex = buffer.ReadInt32FromBuffer(ref offset);
                this.Partitions.Add(partitionIndex);
            }
            this.TagBuffer = buffer.ReadByteFromBuffer(ref offset); // Read the Tag Buffer (1 byte)
        }
    }
}
