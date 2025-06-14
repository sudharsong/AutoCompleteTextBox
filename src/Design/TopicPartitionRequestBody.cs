namespace codecrafterskafka.src.Design
{
    internal class TopicPartitionRequestBody
    {
        private byte[] buffer;
        private int offset;

        public RequestTopic[] Topics
        {
            get;
        }

        public int PartitionLimit
        {
            get;
        }


        public byte? Cursor
        {
            get;
        }

        public byte TagBuffer
        {
            get;
        }

        public TopicPartitionRequestBody(byte[] buffer, int offset)
        {
            this.buffer = buffer;
            this.offset = offset;
            
            var numberOfTopics = this.buffer.ReadByteFromBuffer(ref offset)-1;
            this.Topics = new RequestTopic[numberOfTopics];
            for (int i = 0; i < numberOfTopics; i++)
            {
                var topicNameLength = this.buffer.ReadByteFromBuffer(ref offset);
                var topicName = this.buffer.ReadStringFromBuffer(ref offset, topicNameLength-1);
                var tagBuffer = this.buffer.ReadByteFromBuffer(ref offset);
                this.Topics[i] = new RequestTopic(topicNameLength, topicName, tagBuffer);
            }   


            this.PartitionLimit = this.buffer.ReadInt32FromBuffer(ref offset); // Read the Partition Limit (4 bytes)
            this.Cursor = this.buffer.ReadByteFromBuffer(ref offset); // Read the Cursor (1 byte)
            this.TagBuffer = this.buffer.ReadByteFromBuffer(ref offset); // Read the Tag Buffer (1 byte)
        }
    }
}
