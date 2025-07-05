using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class MetaDataRecord
    {
        public byte FrameVersion { get; set; }

        public RecordType Type { get; set; }

        public byte Version { get; set; }
        public short ValueLength { get; internal set; }
    }
}
