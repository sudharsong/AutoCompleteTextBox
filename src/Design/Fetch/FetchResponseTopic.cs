using codecrafterskafka.src;
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
            Console.WriteLine("TopicID: " + TopicID);
            writer.WriteGuidBigEndian(TopicID);
            writer.WriteVarIntToBuffer(Partitions.Count);
            foreach (var partition in Partitions)
            {
                partition.WriteResponse(writer);
            }

            writer.WriteToBuffer(TagBuffer);
        }
    }
}
