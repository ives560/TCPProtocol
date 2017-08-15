using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtocol
{
    public class RoadGateFrameCodec
    {
        public byte[] FrameBuffer = new byte[1024];
        public int Length = 0;


        /// <summary>
        /// 接收数据帧
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public RoadGateFrame ReceiveFrame(Socket socket)
        {
            int len = socket.Available;
            if (len > 0)//接收Socket数据
            {
                if (len + Length > FrameBuffer.Length)
                {
                    len = FrameBuffer.Length - Length;
                }
                    
                socket.Receive(FrameBuffer, len,SocketFlags.None);
                Length = Length + len;
            }

            //缓存区中有数据,判断帧头
            if (Length > RoadGateFrame.HeardLength)
            {
                if ((FrameBuffer[0] == (byte)RoadGateFrame.Heard && FrameBuffer[1] == (byte)(RoadGateFrame.Heard >> 8))==false)
                {
                    Remove(RoadGateFrame.HeardLength);//移除当前帧长
                    return null;
                }   
            }

            //判断帧数据接收完成
            if ((Length > RoadGateFrame.DataLengthIndex) == false)
            {
                return null;
            }

            //
            int datalen = FrameBuffer[RoadGateFrame.DataLengthIndex];//数据长度
            int framelen = RoadGateFrame.HeardLength + RoadGateFrame.DataLengthIndex + datalen + RoadGateFrame.endLength;//帧长度
            //帧数据接收完成
            if (Length >= framelen)
            {
                if (FrameBuffer[framelen-1] == RoadGateFrame.End)//判断帧尾
                {
                    //校验帧数据
                    if (CheckFrame(FrameBuffer, framelen) == FrameBuffer[framelen - 2])
                    {
                        RoadGateFrame frmae = Decode(FrameBuffer);
                        Remove(framelen);//移除当前帧长
                        return frmae;
                    }
                }

                Remove(framelen);//移除当前帧长
            }

            return null;
        }


        /// <summary>
        /// 帧编码
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public byte[] Encode(RoadGateFrame frame)
        {
            byte[] _byte;
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                writer.Write(frame.heard);
                writer.Write(frame.clientid);
                writer.Write(frame.cmdcode);
                writer.Write(frame.end);
                _byte = mem.ToArray();
                writer.Close();
            }
            return _byte;
        }


        /// <summary>
        /// 帧解码
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public RoadGateFrame Decode(byte[] array)
        {
            RoadGateFrame frame = new RoadGateFrame();
            using (MemoryStream mem = new MemoryStream(array))
            {
                BinaryReader reader = new BinaryReader(mem);

                frame.heard = reader.ReadUInt16();
                frame.length = reader.ReadByte();
                frame.clientid = reader.ReadUInt16();
                frame.cmdtype = reader.ReadByte();
                frame.cmdcode = reader.ReadByte();
                frame.datecount = reader.ReadByte();
                frame.date = reader.ReadBytes(frame.datecount);
                frame.check = reader.ReadByte();
                frame.end = reader.ReadByte();

                reader.Close();

            }
            return frame;
        }

        /// <summary>
        /// 校验帧数据
        /// </summary>
        /// <param name="array"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public UInt16 CheckFrame(byte[] array, int len)
        {

            return (UInt16)XORCheck(array, len);
        }

        /// <summary>
        /// 异或校验
        /// </summary>
        /// <param name="array"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte XORCheck(byte[] array, int len)
        {
            int check = 0;
            for (int i = 2; i < len-2;i++ )
            {
                check = check ^ array[i];
            }
            return (byte)check;
        }


        /// <summary>
        /// 从Buffer中移除数据
        /// </summary>
        /// <param name="count"></param>
        private void Remove(int count)
        {
            if (Length >= count)
            {
                Buffer.BlockCopy(FrameBuffer, count, FrameBuffer, 0, Length - count);
                Length -= count;
            }
            else
            {
                Length = 0;
            }
        }
    }
}
