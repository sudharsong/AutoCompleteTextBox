using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class RecordBatch
    {
        private byte[] crcData;

        public int BatchStartOffset { get; set; }
        // Batch header fields
        public long BaseOffset { get; set; }
        public int BatchLength { get; set; }
        public int PartitionLeaderEpoch { get; set; }
        public byte MagicByte { get; set; }
        public int Crc { get; set; }
        public short Attributes { get; set; }
        public int LastOffsetDelta { get; set; }
        public long BaseTimestamp { get; set; }
        public long MaxTimestamp { get; set; }
        public long ProducerId { get; set; }
        public short ProducerEpoch { get; set; }
        public int BaseSequence { get; set; }
        public int RecordsLength { get; set; }

        public ArraySegment<byte> CRCData { get; set; }

        // The actual records in this batch
        public List<Record> Records { get; set; } = new();
        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            //Console.WriteLine($"BaseOffset: {BaseOffset} {Environment.NewLine} BatchLength: {BatchLength} {Environment.NewLine} PartitionLeaderEpoch: {PartitionLeaderEpoch} {Environment.NewLine} MagicByte: {MagicByte} {Environment.NewLine} Crc: {Crc} {Environment.NewLine}Attributes: {Attributes} {Environment.NewLine}LastOffsetDelta: {LastOffsetDelta} {Environment.NewLine}BaseTimestamp: {BaseTimestamp} {Environment.NewLine}MaxTimestamp: {MaxTimestamp} {Environment.NewLine}ProducerId: {ProducerId} {Environment.NewLine}ProducerEpoch: {ProducerEpoch} {Environment.NewLine}BaseSequence: {BaseSequence}");
            writer.WriteToBuffer(BaseOffset);
            writer.WriteToBuffer(BatchLength);
            writer.WriteToBuffer(PartitionLeaderEpoch);
            writer.WriteToBuffer(MagicByte);
            this.Crc = this.CRCData.CalculateCRCforRecordBatch();
            writer.WriteToBuffer(this.Crc);

            //var crcSpan = writer.GetSpan(4);
            //writer.Advance(4); // reserve space for length  

            //int crcFieldStart = BatchLength + 4 + 1;
            //int crcDataStart = crcFieldStart + 4;
            //int crcDataLen = BatchLength - (1 + 4);

            //if (crcDataLen <= 0)
            //    {
            //    throw new InvalidOperationException("Invalid batch length for CRC");
            //}

            //uint checksum = Crc32.Hash(buffer.AsSpan(crcDataStart, crcDataLen));
            //BinaryPrimitives.WriteUInt32BigEndian(
            //    buffer.AsSpan(crcFieldStart, 4),
            //    checksum);


            //writer.WriteToBuffer(Crc);            
            writer.WriteToBuffer(Attributes);
            writer.WriteToBuffer(LastOffsetDelta);
            writer.WriteToBuffer(BaseTimestamp);
            writer.WriteToBuffer(MaxTimestamp);
            writer.WriteToBuffer(ProducerId);
            writer.WriteToBuffer(ProducerEpoch);
            writer.WriteToBuffer(BaseSequence);
            writer.WriteToBuffer((Records.Count() + 1));
            //BinaryPrimitives.WriteUInt32BigEndian(crcSpan, 0);
            foreach (var record in Records)
            {
                Console.WriteLine($"Processing Record with Length: {record.Length}"); 
                record.WriteResponse(writer);
            }

            //Console.WriteLine($"Calling Patch");
            //Console.WriteLine($"BatchStartOffset: {BatchStartOffset} BatchLength: {BatchLength}");
            //var crc = writer.WrittenSpan.ToArray().PatchRecordBatchCrc(this.BatchStartOffset, BatchLength);
            Console.WriteLine($"Crc: {Crc}");   
           // BinaryPrimitives.WriteInt32BigEndian(crcSpan, this.Crc);
        }

        public override string ToString()
        {
            return $"BaseOffset: {BaseOffset} {Environment.NewLine} BatchLength: {BatchLength} {Environment.NewLine} PartitionLeaderEpoch: {PartitionLeaderEpoch} {Environment.NewLine} MagicByte: {MagicByte} {Environment.NewLine} Crc: {Crc} {Environment.NewLine}Attributes: {Attributes} {Environment.NewLine}LastOffsetDelta: {LastOffsetDelta} {Environment.NewLine}BaseTimestamp: {BaseTimestamp} {Environment.NewLine}MaxTimestamp: {MaxTimestamp} {Environment.NewLine}ProducerId: {ProducerId} {Environment.NewLine}ProducerEpoch: {ProducerEpoch} {Environment.NewLine}BaseSequence: {BaseSequence} {Environment.NewLine} Reconrd Length {this.RecordsLength} ";

        }
    }
}
