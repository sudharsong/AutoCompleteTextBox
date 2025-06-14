using System;

namespace codecrafterskafka.src.Design
{
    internal class TopicPartitionRequestBodyV0 : RequestBody
    {
        private int numberOfTopics;
        //private byte[] buffer;

        public RequestTopic[] Topics
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
            this.numberOfTopics = buffer.ReadByteFromBuffer(ref offset) - 1;            
            this.Topics = new RequestTopic[numberOfTopics];
            for (int i = 0; i < numberOfTopics; i++)
            {
                var topicNameLength = buffer.ReadByteFromBuffer(ref offset);
                var topicName = buffer.ReadStringFromBuffer(ref offset, topicNameLength - 1);
                var tagBuffer = buffer.ReadByteFromBuffer(ref offset);
                this.Topics[i] = new RequestTopic(topicNameLength, topicName, tagBuffer);
            }

            this.PartitionLimit = buffer.ReadInt32FromBuffer(ref offset); // Read the Partition Limit (4 bytes)
            this.Cursor = buffer.ReadByteFromBuffer(ref offset); // Read the Cursor (1 byte)
            this.TagBuffer = buffer.ReadByteFromBuffer(ref offset); // Read the Tag Buffer (1 byte)    
        }
    }
}
