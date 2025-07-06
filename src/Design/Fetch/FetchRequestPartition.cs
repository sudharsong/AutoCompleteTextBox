using codecrafterskafka.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Fetch
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
            PartitionIndex = buffer.ReadInt32FromBuffer(ref offset);
            CurrentLeaderEpoch = buffer.ReadInt32FromBuffer(ref offset);
            FetchOffset = buffer.ReadInt64FromBuffer(ref offset);
            LastFetchedEpoch = buffer.ReadInt32FromBuffer(ref offset);
            LogStartOffset = buffer.ReadInt64FromBuffer(ref offset);
            MaxBytes = buffer.ReadInt32FromBuffer(ref offset);
            TagBuffer = buffer.ReadByteFromBuffer(ref offset);
        }
    }

    internal class ForgottenTopicPartitions
    {
        public Guid TopicID { get; set; }
        public List<int> Partitions { get; set; } = new List<int>();
        public byte TagBuffer { get; set; }
        public void PopulateBody(byte[] buffer, int offset)
        {
            TopicID = buffer.ReadGuidFromBuffer(ref offset);
            uint numberOfPartitions = buffer.ReadUVarInt(ref offset);
            for (int i = 0; i < numberOfPartitions - 1; i++)
            {
                var partitionIndex = buffer.ReadInt32FromBuffer(ref offset);
                Partitions.Add(partitionIndex);
            }
            TagBuffer = buffer.ReadByteFromBuffer(ref offset); // Read the Tag Buffer (1 byte)
        }
    }
}
