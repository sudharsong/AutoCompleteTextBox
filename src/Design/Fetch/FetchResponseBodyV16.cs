using codecrafterskafka.src;
using src.Design.Base;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Fetch
{
    internal class FetchResponseBodyV16 : ResponseBody
    {
        public int ThrottleTime { get; set; }

        public short ErrorCode { get; set; }

        public int SessionId { get; set; }

        public byte TagBuffer { get; set; }

        public byte PartitionsCount
        {
            get; set;
        }

        public List<FetchResponseTopic> Responses { get; set; }

        public override void WriteResponse(ArrayBufferWriter<byte> writer)
        {
            writer.WriteToBuffer(ThrottleTime);
            writer.WriteToBuffer(ErrorCode);
            writer.WriteToBuffer(SessionId);
            writer.WriteUVarInt((uint)(Responses.Count+1));//need to comment
            foreach (var response in Responses)
            {
                response.WriteResponse(writer);
            }

            writer.WriteToBuffer(TagBuffer);
        }
    }
}
