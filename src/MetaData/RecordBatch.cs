using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class RecordBatch
    {
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

        // The actual records in this batch
        public List<Record> Records { get; set; } = new();
    }
}
