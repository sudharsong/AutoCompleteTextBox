using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal class FeatureLevelRecord : MetaDataRecord
    {
        public uint NameLength { get; set; }    

        public string? Name { get; set; }

        public short FeatureLevel { get; set; }

        public uint TaggedFieldCount { get; set; }  

    }
}
