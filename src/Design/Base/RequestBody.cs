using System;

namespace src.Design.Base
{
    internal abstract class RequestBody
    {
        public abstract void PopulateBody(byte[] buffer, int offset);
    }
}