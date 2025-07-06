using codecrafterskafka.src;
using src.Design.Base;
using System;

namespace src.Design.TopicPartition
{
    internal class TopicPartitionRequestBodyV0 : RequestBody
    {
        private int numberOfTopics;
        //private byte[] buffer;

        public RequestTopic[]? Topics
        {
            get; private set;
        }

        public int PartitionLimit
        {
            get; private set;
        }


        public byte? Cursor
        {
            get; private set;
        }

        public byte TagBuffer
        {
            get; private set;
        }

        public override void PopulateBody(byte[] buffer, int offset)
        {      
            numberOfTopics = buffer.ReadByteFromBuffer(ref offset) - 1;            
            Topics = new RequestTopic[numberOfTopics];
            for (int i = 0; i < numberOfTopics; i++)
            {
                var topicNameLength = buffer.ReadByteFromBuffer(ref offset);
                var topicName = buffer.ReadStringFromBuffer(ref offset, topicNameLength - 1);
                var tagBuffer = buffer.ReadByteFromBuffer(ref offset);
                Topics[i] = new RequestTopic(topicNameLength, topicName, tagBuffer);
            }

            PartitionLimit = buffer.ReadInt32FromBuffer(ref offset); // Read the Partition Limit (4 bytes)
            Cursor = buffer.ReadByteFromBuffer(ref offset); // Read the Cursor (1 byte)
            TagBuffer = buffer.ReadByteFromBuffer(ref offset); // Read the Tag Buffer (1 byte)    
        }
    }
}
