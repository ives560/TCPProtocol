using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtocol.protocol
{
    class ETCFrameCodec
    {
        byte[] Encode(ETCFrame frame)
        {
            return new byte[1];
        }

        ETCFrame Decode(byte[] array)
        {
            return new ETCFrame();
        }
    }
}
