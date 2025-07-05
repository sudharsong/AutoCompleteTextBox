using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafterskafka.src.MetaData
{
    internal enum RecordType
    {
        FeatureLevel = 0x0c,
        Topic = 0x02,
        Partition = 0x03
    }
}
