using codecrafterskafka.src;
using codecrafterskafka.src.MetaData;
using src.Design.TopicPartition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace src.MetaDatakafka.src
{
    internal class LogMetaData
    {
        private List<RecordBatch> recordBatches = new List<RecordBatch>();
        private List<PartitionRecord>? partitionRecords;
        private List<TopicRecord>? topicRecords;

        private LogMetaData()
        {
            // Private constructor to prevent instantiation
        }

        private static readonly Lazy<LogMetaData> instance = new Lazy<LogMetaData>(() => new LogMetaData());
        public static LogMetaData Instance => instance.Value;

        public async Task LoadLogMetaDataAsync(string logMetadataFilePath, CancellationToken token)
        {
            partitionRecords = new List<PartitionRecord>();
            topicRecords = new List<TopicRecord>();
            Console.WriteLine($"Loading log metadata from: {logMetadataFilePath}");
            var fileBuffer = await File.ReadAllBytesAsync(logMetadataFilePath, token);
            ReadRecordBatches(fileBuffer, 0);
        }
        
        private void ReadRecordBatches(byte[] buffer, int startLength)
        {
            if (startLength >= buffer.Length)
            {
                return;
            }

            int currentLength = startLength;
            RecordBatch batch = new RecordBatch();
            batch.BatchStartOffset = currentLength;
            batch.BaseOffset = buffer.ReadInt64FromBuffer(ref currentLength);
            batch.BatchLength = buffer.ReadInt32FromBuffer(ref currentLength);            
            batch.PartitionLeaderEpoch = buffer.ReadInt32FromBuffer(ref currentLength);
            batch.MagicByte = buffer.ReadByteFromBuffer(ref currentLength);
            Console.WriteLine($"Batch Length: {batch.BatchLength} Current Length: {currentLength} Buffer Length: {buffer.Length}");
            //batch.Crc = buffer.CalculateCrcForBatch(currentLength, currentLength +  batch.BatchLength - 5);
            batch.CRCData = buffer.GetCRCData(currentLength, currentLength + batch.BatchLength - 5);
            batch.Crc = batch.CRCData.CalculateCRCforRecordBatch();
            currentLength += 4; // Move past the CRC field
            //Console.WriteLine($"Crc: {Encoding.UTF8.GetString(batch.Crc)} Current Length: {currentLength} Buffer Length: {buffer.Length}");
            batch.Attributes = buffer.ReadInt16FromBuffer(ref currentLength);
            batch.LastOffsetDelta = buffer.ReadInt32FromBuffer(ref currentLength);
            batch.BaseTimestamp = buffer.ReadInt64FromBuffer(ref currentLength);
            batch.MaxTimestamp = buffer.ReadInt64FromBuffer(ref currentLength);
            batch.ProducerId = buffer.ReadInt64FromBuffer(ref currentLength);
            batch.ProducerEpoch = buffer.ReadInt16FromBuffer(ref currentLength);
            batch.BaseSequence = buffer.ReadInt32FromBuffer(ref currentLength);
            batch.RecordsLength = buffer.ReadInt32FromBuffer(ref currentLength);
            for (int i = 0; i < batch.RecordsLength; i++)
            {
                Record record = new Record();
                record.Length = buffer.ReadVarInt(ref currentLength);
                record.Attributes = buffer.ReadByteFromBuffer(ref currentLength);
                record.TimestampDelta = buffer.ReadVarInt(ref currentLength);
                record.OffsetDelta = buffer.ReadVarInt(ref currentLength);
                record.KeyLength = buffer.ReadVarInt(ref currentLength);
                if (record.KeyLength > 0)
                {
                    record.Key = buffer.ReadStringFromBuffer(ref currentLength, record.KeyLength);
                }


                record.ValueLength = buffer.ReadVarInt(ref currentLength);
                byte frameVersion = buffer.ReadByteFromBuffer(ref currentLength);
                RecordType recordType = (RecordType)buffer.ReadByteFromBuffer(ref currentLength);

                byte version = buffer.ReadByteFromBuffer(ref currentLength);
                MetaDataRecord recordValue = recordType == RecordType.FeatureLevel ?
                    ReadFeatureRecod(buffer, ref currentLength) :
                    recordType == RecordType.Partition ?
                    ReadPartitionRecord(buffer, ref currentLength) :
                    ReadTopicRecord(buffer, ref currentLength);

                recordValue.Version = version;
                recordValue.FrameVersion = frameVersion;
                recordValue.Type = recordType;
                record.Value = recordValue;
                record.HeaderArrayCount = buffer.ReadUVarInt(ref currentLength);
                batch.Records.Add(record);
            }

            this.recordBatches.Add(batch);
            ReadRecordBatches(buffer, currentLength);
        }

        private MetaDataRecord ReadTopicRecord(byte[] buffer, ref int currentOffset)
        {
            TopicRecord record = new TopicRecord();
            record.NameLength = buffer.ReadUVarInt(ref currentOffset);
            record.Name = buffer.ReadStringFromBuffer(ref currentOffset, Convert.ToInt32(record.NameLength - 1));
            record.TopicUUID = buffer.ReadGuidFromBuffer(ref currentOffset);
            record.TaggedFieldCount = buffer.ReadUVarInt(ref currentOffset);
            this.topicRecords.Add(record);
            return record;
        }

        private MetaDataRecord ReadPartitionRecord(byte[] buffer, ref int currentOffset)
        {
            PartitionRecord record = new PartitionRecord();
            record.ParititionId = buffer.ReadInt32FromBuffer(ref currentOffset);
            record.TopicUUID = buffer.ReadGuidFromBuffer(ref currentOffset);
            record.ReplicaArrayLength = buffer.ReadUVarInt(ref currentOffset);

            for (int i = 0; i < record.ReplicaArrayLength - 1; i++)
            {
                if (record.ReplicaArray == null)
                {
                    record.ReplicaArray = new int[record.ReplicaArrayLength - 1];
                }
                record.ReplicaArray[i] = buffer.ReadInt32FromBuffer(ref currentOffset);
            }

            record.SyncReplicaArrayLength = buffer.ReadUVarInt(ref currentOffset);
            for (int i = 0; i < record.SyncReplicaArrayLength - 1; i++)
            {
                if (record.SyncReplicaArray == null)
                {
                    record.SyncReplicaArray = new int[record.SyncReplicaArrayLength - 1];
                }

                record.SyncReplicaArray[i] = buffer.ReadInt32FromBuffer(ref currentOffset);
            }

            record.RemovingReplicaArrayLength = buffer.ReadUVarInt(ref currentOffset) - 1;
            record.AddingReplicaArrayLength = buffer.ReadUVarInt(ref currentOffset) - 1;
            record.Leader = buffer.ReadInt32FromBuffer(ref currentOffset);
            record.LeaderEpoch = buffer.ReadInt32FromBuffer(ref currentOffset);
            record.PartitionEpoch = buffer.ReadInt32FromBuffer(ref currentOffset);
            record.DirectoriesArrayLength = buffer.ReadUVarInt(ref currentOffset);
            for (int i = 0; i < record.DirectoriesArrayLength - 1; i++)
            {
                if (record.DirectoriesArray == null)
                {
                    record.DirectoriesArray = new Guid[record.DirectoriesArrayLength - 1];
                }

                record.DirectoriesArray[i] = buffer.ReadGuidFromBuffer(ref currentOffset);
            }

            record.TaggedFieldCount = buffer.ReadUVarInt(ref currentOffset);
            this.partitionRecords.Add(record);
            return record;
        }

        private MetaDataRecord ReadFeatureRecod(byte[] buffer, ref int currentOffset)
        {
            FeatureLevelRecord record = new FeatureLevelRecord();
            record.NameLength = buffer.ReadUVarInt(ref currentOffset);
            record.Name = buffer.ReadStringFromBuffer(ref currentOffset, Convert.ToInt32(record.NameLength - 1));
            record.FeatureLevel = buffer.ReadInt16FromBuffer(ref currentOffset);
            record.TaggedFieldCount = buffer.ReadUVarInt(ref currentOffset);
            return record;
        }

        public List<TopicPartitions> GetTopicPartitions(RequestTopic[]? topics)
        {
            List<TopicPartitions> topicPartitions = new List<TopicPartitions>();
            foreach (var topic in topics)
            {
                TopicPartitions partitions = new TopicPartitions();
                partitions.TopicName = topic.Name;
                partitions.TopicID = this.topicRecords.FirstOrDefault(t => t.Name == topic.Name)?.TopicUUID ?? Guid.Empty;
                var partitionRecords = this.partitionRecords.Where(p => p.TopicUUID == partitions.TopicID).ToList();
                partitions.PartitionCount = partitionRecords.Count;
                partitions.PartitionIndexes = partitionRecords.Select(p => p.ParititionId).ToArray();
                partitions.Partitions = partitionRecords.Select(a => new Partition
                {
                    PartitionIndex = a.ParititionId,
                    ErrorCode = 0, // Assuming no error for simplicity
                    LeaderID = a.Leader,
                    LeaderEpoch = a.LeaderEpoch,
                    ReplicaNodes = a.ReplicaArray ?? Array.Empty<int>(),
                    ReplicaNodesLength = (byte)(a.ReplicaArray?.Length + 1 ?? 0),
                    ISRNodes = a.SyncReplicaArray ?? Array.Empty<int>(),
                    ISRNodesLength = (byte)(a.SyncReplicaArray?.Length + 1 ?? 0),
                    OfflineReplicaNodesLength = Convert.ToInt32(a.RemovingReplicaArrayLength),
                    EligibleLeaderReplicaNodesLength = Convert.ToInt32(a.AddingReplicaArrayLength),
                    LastKnownELRLength = 0, // Assuming no last known eligible leader replica nodes for simplicity
                    TagBuffer = 0
                }).ToArray();
                topicPartitions.Add(partitions);
            }

            return topicPartitions;
        }

        public TopicRecord GetTopicRecord(Guid topicId)
        {
            return this.topicRecords.FirstOrDefault(t => t.TopicUUID.Equals(topicId));
        }

        public bool IsTopicPartitionExist(Guid topicId)
        {
            return this.partitionRecords.Any(p => p.TopicUUID.Equals(topicId));
        }

        public List<RecordBatch> GetPartitionRecordBatches(Guid topicID)
        {
            var result = this.recordBatches
                .Where(a => a.Records.Any(r => r.Value is PartitionRecord pr && pr.TopicUUID == topicID)).ToList();
            //foreach (var batch in result)
            //{
            //   Console.WriteLine(batch.ToString());
            //}

            return result;
        }
    }
}
