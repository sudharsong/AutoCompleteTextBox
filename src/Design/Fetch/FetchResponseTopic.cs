using codecrafterskafka.src;
using src.Design.TopicPartition;
using src.MetaDatakafka.src;
using System.Buffers;

namespace src.Design.Fetch
{
    internal class FetchResponseTopic
    {
        public Guid TopicID { get; set; }

        public List<FetchResponseTopicPartition> Partitions
        {
            get; set;
        } = new List<FetchResponseTopicPartition>();

        public byte TagBuffer { get; set; } 

        public void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            var topicName = LogMetaData.Instance.GetTopicRecord(TopicID)?.Name;
            //  Console.WriteLine("TopicID: " + TopicID);
            writer.WriteGuidToBuffer(TopicID);
            //writer.WriteVarIntToBuffer(Partitions.Count);
            writer.WriteUVarInt((uint)(Partitions.Count+1));
            Console.WriteLine("Partitions Count: " + Partitions.Count);
            foreach (var partition in Partitions)
            {
                partition.TopicName = topicName;    
                partition.WriteResponse(writer);
            }

            writer.WriteToBuffer(TagBuffer);
        }
    }
}
