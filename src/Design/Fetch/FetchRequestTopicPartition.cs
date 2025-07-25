﻿using codecrafterskafka.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Fetch
{
    internal class FetchRequestTopicPartition
    {
        public Guid TopicID { get; set; }

        public List<FetchRequestPartition> Partitions
        {
            get; set;
        } = new List<FetchRequestPartition>();

        public byte TagBuffer { get; set; }

        public void PopulateBody(byte[] buffer, int offset)
        {
            TopicID = buffer.ReadGuidFromBuffer(ref offset);
            uint numberOfPartitions = buffer.ReadUVarInt(ref offset);
            for (int i = 0; i < numberOfPartitions - 1; i++)
            {
                var partition = new FetchRequestPartition();
                partition.PopulateBody(buffer, offset);
                Partitions.Add(partition);
            }
            TagBuffer = buffer.ReadByteFromBuffer(ref offset); // Read the Tag Buffer (1 byte)
        }
    }
}
