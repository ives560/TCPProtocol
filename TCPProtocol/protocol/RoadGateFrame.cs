using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtocol
{
    public class RoadGateFrame
    {
        /// <summary>
        /// 数据长度在帧中的位置
        /// </summary>
        public static readonly int DataLengthIndex = 7;
        /// <summary>
        /// 帧头长度
        /// </summary>
        public static readonly int HeardLength = 2;
        /// <summary>
        /// 帧尾长度
        /// </summary>
        public static readonly int endLength = 1;
        /// <summary>
        /// 帧头数据
        /// </summary>
        public static readonly UInt16 Heard = 0xffff;
        /// <summary>
        /// 帧尾数据
        /// </summary>
        public static readonly byte End = 0xff;

        //完整帧数据
        public UInt16 heard = Heard;
        public byte length;
        public UInt16 clientid;
        public byte cmdtype;
        public byte cmdcode;
        public byte datecount;
        public byte[] date;
        public byte check;
        public byte end = End;
    }
}
