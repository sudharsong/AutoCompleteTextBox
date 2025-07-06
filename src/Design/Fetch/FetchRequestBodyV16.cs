using codecrafterskafka.src;
using src.Design.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Fetch
{
    internal class FetchRequestBodyV16 : RequestBody
    {
        public int MaxWaitTime { get; private set; }

        public int MinBytes { get; private set; }

        public int MaxBytes { get; private set; }

        public byte IsolationLevel { get; private set; }

        public int SessionID { get; private set; }

        public int SessionEpoch { get; private set; }

        public List<FetchRequestTopicPartition> Topics { get; private set; } = new List<FetchRequestTopicPartition>();

        public List<ForgottenTopicPartitions> ForgottenTopics { get; private set; } = new List<ForgottenTopicPartitions>();

        public string? RackID { get; private set; }

        public override void PopulateBody(byte[] buffer, int offset)
        {
            MaxWaitTime = buffer.ReadInt32FromBuffer(ref offset);
            MinBytes = buffer.ReadInt32FromBuffer(ref offset);
            MaxBytes = buffer.ReadInt32FromBuffer(ref offset);
            IsolationLevel = buffer.ReadByteFromBuffer(ref offset);
            SessionID = buffer.ReadInt32FromBuffer(ref offset);
            SessionEpoch = buffer.ReadInt32FromBuffer(ref offset);
            byte numberOfTopics = buffer.Length - offset < 16 ? (byte)0 : buffer.ReadByteFromBuffer(ref offset);
            for (int i = 0; i < numberOfTopics-1 ; i++)
            {
                var topicPartition = new FetchRequestTopicPartition();
                topicPartition.PopulateBody(buffer, offset);
                this.Topics.Add(topicPartition);
            }

            byte numberOfForgottenTopics = buffer.Length - offset < 16 ? (byte)0 :  buffer.ReadByteFromBuffer(ref offset);
            for (int i = 0; i < numberOfForgottenTopics-1; i++)
            {
                var forgottenTopic = new ForgottenTopicPartitions();
                forgottenTopic.PopulateBody(buffer, offset);
                this.ForgottenTopics.Add(forgottenTopic);
            }

            RackID = buffer.ReadStringFromBuffer(ref offset, buffer.Length - offset);
        }
    }
}
