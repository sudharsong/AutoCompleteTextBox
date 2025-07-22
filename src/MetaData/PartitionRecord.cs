using System;
using System.Buffers;
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

        public override void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            base.WriteResponse(writer);
            writer.WriteToBuffer(ParititionId);
            writer.WriteGuidToBuffer(TopicUUID);
            writer.WriteVarIntToBuffer((int)ReplicaArrayLength);
            if (ReplicaArray != null)
            {
                foreach (var replica in ReplicaArray)
                {
                    writer.WriteToBuffer(replica);
                }
            }

            writer.WriteVarIntToBuffer((int)SyncReplicaArrayLength);
            if (SyncReplicaArray != null)
            {
                foreach (var syncReplica in SyncReplicaArray)
                {
                    writer.WriteToBuffer(syncReplica);
                }
            }

            writer.WriteVarIntToBuffer((int)RemovingReplicaArrayLength);
            writer.WriteVarIntToBuffer((int)AddingReplicaArrayLength);
            writer.WriteToBuffer(Leader);
            writer.WriteToBuffer(LeaderEpoch);
            writer.WriteToBuffer(PartitionEpoch);
            writer.WriteVarIntToBuffer((int)DirectoriesArrayLength);
            if (DirectoriesArray != null)
            {
                foreach (var directory in DirectoriesArray)
                {
                    writer.WriteGuidToBuffer(directory);
                }
            }

            writer.WriteVarIntToBuffer((int)TaggedFieldCount);
        }
    }
}
