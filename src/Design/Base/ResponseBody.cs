using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Design.Base
{
    internal abstract class ResponseBody
    {
        public abstract void WriteResponse(ArrayBufferWriter<byte> writer);
    }
}
