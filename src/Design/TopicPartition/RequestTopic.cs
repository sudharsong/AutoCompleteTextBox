using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.TopicPartition
{
    internal class RequestTopic
    {
        public byte NameLength { get;  }

        public string Name { get;  }

        public byte TopicTagBuffer { get; }    

        public RequestTopic(byte nameLength, string name, byte topicTagBuffer)
        {
            NameLength = nameLength;
            Name = name;
            TopicTagBuffer = topicTagBuffer;
        }
    }
}
