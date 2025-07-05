using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class PartitionRecord : MetaDataRecord
    {
        public int ParititionId { get; set; }   

        public Guid TopicUUID { get; set; }

        public uint ReplicaArrayLength { get; set; }

        public int[]? ReplicaArray { get; set; }

        public uint SyncReplicaArrayLength { get; set; }

        public int[]? SyncReplicaArray { get; set; }

        public uint RemovingReplicaArrayLength { get; set; }

        public uint AddingReplicaArrayLength { get; set; }

        public int Leader {  get; set; }

        public int LeaderEpoch { get; set; }

        public int PartitionEpoch { get; set; }

        public uint DirectoriesArrayLength { get; set; }

        public Guid[]? DirectoriesArray { get; set; }

        public uint TaggedFieldCount { get; set; }

    }
}
