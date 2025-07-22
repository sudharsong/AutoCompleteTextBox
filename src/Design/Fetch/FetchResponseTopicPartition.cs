using codecrafterskafka.src;
using codecrafterskafka.src.MetaData;
using src.Design.TopicPartition;
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

        public List<RecordBatch> Records { get; internal set; }

        public byte TagBuffer { get; internal set; }

        public string TopicName { get; set; }  

        public byte RecordsLength
        {
            get
            {
                if (this.Records == null)
                {
                    return 0;
                }
                else {
                    return (byte)this.Records.Count();
                }
            }
        }

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(PartitionIndex);
            writer.WriteToBuffer(ErrorCode);
            writer.WriteToBuffer(HighWatermark);
            writer.WriteToBuffer(LastStableOffset);
            writer.WriteToBuffer(LogStartOffset);
            writer.WriteToBuffer((byte)(AbortedTransactions.Length + 1));
            foreach (var aborted in this.AbortedTransactions)
            {
                aborted.WriteResponse(writer);
            }

            writer.WriteToBuffer(PreferredReadReplica);

            string partitionLogPath = "/tmp/kraft-combined-logs/" + this.TopicName +
                                  "-" + PartitionIndex +
                                  "/00000000000000000000.log";
            if (File.Exists(partitionLogPath))
            {
                byte[] record = File.ReadAllBytes(partitionLogPath);
                writer.WriteVarIntToBuffer(record.Length);
                writer.WriteToBuffer(record);
            }
            else
            {
                writer.WriteVarIntToBuffer(0 + 1);
            }
            //writer.WriteToBuffer((byte)(this.Records.Sum(a => a.BatchLength + 12)));
            //Console.WriteLine($"Records Length inside: {this.RecordsLength}");
            //writer.WriteToBuffer(Records);
            //for (int i = 0;i < RecordsLength; i++)
            //{
            //    Console.WriteLine($"Record Length {Records[i].BatchLength + 12}");
            //    writer.WriteUVarInt((uint)(Records[i].BatchLength + 12+1));
            //    Records[i].WriteResponse(writer);
            //}

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
