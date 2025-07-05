using codecrafterskafka.src.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class TopicPartitions
    {
        public bool IsValidTopic
        {
            get;set;
        }
        public string TopicName { get; set; } = string.Empty;

        public int PartitionCount { get; set; } = 0;

        public int[] PartitionIndexes { get; set; } = Array.Empty<int>();   

        public Partition[] Partitions { get; set; } = Array.Empty<Partition>(); 

        public Guid TopicID { get; set; } = Guid.Empty;
    }
}
