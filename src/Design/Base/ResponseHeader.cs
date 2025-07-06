using System.Buffers;

namespace src.Design.Base
{
    internal abstract class ResponseHeader
    {
        public int CorrelationId { get; }

        public ResponseHeader(int correlationId)
        {
            CorrelationId = correlationId;
        }

        public abstract void WriteResponse(ArrayBufferWriter<byte> writer);
    }
}