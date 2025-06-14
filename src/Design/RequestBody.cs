using System;

namespace codecrafterskafka.src.Design
{
    internal abstract class RequestBody
    {
        public abstract void PopulateBody(byte[] buffer, int offset);
    }
}