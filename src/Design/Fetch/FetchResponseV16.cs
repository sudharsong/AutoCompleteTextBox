using System;
using System.Buffers.Binary;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Design.Base;

namespace src.Design.Fetch
{
    internal class FetchResponseV16 : Response
    {
        public FetchResponseV16(FetchResponseHeaderV16 header, FetchResponseBodyV16 body)
        {
            Header = header;
            Body = body;
        }
    }
}
