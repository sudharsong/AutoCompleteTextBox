using codecrafterskafka.src;
using System.Buffers;

namespace src.Design.Fetch
{
    internal class FetchResponseTopicPartition
    {
        public int PartitionIndex { get; internal set; }
        public short ErrorCode { get; internal set; }
        public long HighWatermark { get; internal set; }
        public long LastStableOffset { get; internal set; }
        public long LogStartOffset { get; internal set; }
        public AbortedTransaction[] AbortedTransactions { get; internal set; } = Array.Empty<AbortedTransaction>();

        public int PreferredReadReplica { get; internal set; } = 0;

        public byte[] Records { get; internal set; } = Array.Empty<byte>();

        public byte TagBuffer { get; internal set; }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(PartitionIndex);
            writer.WriteToBuffer(ErrorCode);
            writer.WriteToBuffer(HighWatermark);
            writer.WriteToBuffer(LastStableOffset);
            writer.WriteToBuffer(LogStartOffset);
            writer.WriteVarIntToBuffer(AbortedTransactions.Length);
            foreach (var aborted in this.AbortedTransactions)
            {
                aborted.WriteResponse(writer);
            }

            writer.WriteToBuffer(PreferredReadReplica);
            writer.WriteVarIntToBuffer(Records.Length);
            writer.WriteToBuffer(Records);
            writer.WriteToBuffer(TagBuffer);
        }
    }

    internal class AbortedTransaction
    {
        public long ProducerId { get; internal set; }

        public long FirstOffset { get; internal set; }

        public byte TagBuffer { get; internal set; }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(ProducerId);
            writer.WriteToBuffer(FirstOffset);  
            writer.WriteToBuffer(TagBuffer);
        }
    }
}
