using codecrafterskafka.src.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class Record
    {
        public int Length { get; set; }
        public byte Attributes { get; set; }
        public int TimestampDelta { get; set; }
        public int OffsetDelta { get; set; }
        public string? Key { get; set; }

        public int ValueLength { get; set; }

        public MetaDataRecord? Value { get; set; }

        public uint HeaderArrayCount { get; set; }
        public int KeyLength { get; internal set; }
    }
}
