using codecrafterskafka.src;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.TopicPartition
{
    internal class Partition
    {
        public short ErrorCode { get; set; }

        public int PartitionIndex { get; set; }

        public int LeaderID { get; set; }

        public int LeaderEpoch { get; set; }

        public byte ReplicaNodesLength { get; set; }

        public int[]? ReplicaNodes { get; set; }

        public byte ISRNodesLength { get; set; }

        public int[]? ISRNodes { get; set; }

        public int EligibleLeaderReplicaNodesLength { get; set; }

        //public int[]? EligibleLeaderReplicaNodes { get; set; }

        public int LastKnownELRLength { get; set; }

        //public int[]? LastKnownELR { get; set; }

        public int OfflineReplicaNodesLength { get; set; }

        //public int[]? OfflineReplicaNodes { get; set; }

        public byte TagBuffer { get; set; }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(ErrorCode);
            writer.WriteToBuffer(PartitionIndex);
            writer.WriteToBuffer(LeaderID);
            writer.WriteToBuffer(LeaderEpoch);
            // Write Replica Nodes
            writer.WriteToBuffer(ReplicaNodesLength);
            if (ReplicaNodes != null)
            {
                foreach (var node in ReplicaNodes)
                {
                    writer.WriteToBuffer(node);
                }
            }
            // Write ISR Nodes
            writer.WriteToBuffer(ISRNodesLength);
            if (ISRNodes != null)
            {
                foreach (var node in ISRNodes)
                {
                    writer.WriteToBuffer(node);
                }
            }
            // Write Eligible Leader Replica Nodes
            writer.WriteVarIntToBuffer(EligibleLeaderReplicaNodesLength);
            //if (this.EligibleLeaderReplicaNodes != null)
            //{
            //    foreach (var node in this.EligibleLeaderReplicaNodes)
            //    {
            //        writer.WriteInt32ToBuffer(node);
            //    }
            //}

            // Write Last Known ELR
            writer.WriteVarIntToBuffer(LastKnownELRLength);
            //if (this.LastKnownELR != null)
            //{
            //    foreach (var node in this.LastKnownELR)
            //    {
            //        writer.WriteInt32ToBuffer(node);
            //    }
            //}

            // Write Offline Replica Nodes
            writer.WriteVarIntToBuffer(OfflineReplicaNodesLength);
            //if (this.OfflineReplicaNodes != null)
            //{
            //    foreach (var node in this.OfflineReplicaNodes)
            //    {
            //        writer.WriteInt32ToBuffer(node);
            //    }
            //}

            // Write Tag Buffer
            writer.WriteToBuffer(TagBuffer);
        }
    }
}
