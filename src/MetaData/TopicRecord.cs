using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class TopicRecord : MetaDataRecord
    {
        public uint NameLength { get; set; }

        public string? Name { get; set; }

        public Guid TopicUUID { get; set; }

        public uint TaggedFieldCount { get; set; }
    }
}
