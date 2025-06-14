namespace codecrafterskafka.src.Design
{
    internal class Header
    {
        public int CorrelationId { get; }

        public Header(int correlationId)
        {
            this.CorrelationId = correlationId;
        }   
    }
}