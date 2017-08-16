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
                        RoadGateFrame frmae = Decode(FrameBuffer, framelen);
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
            return EnEscape(_byte, _byte.Length);
        }


        /// <summary>
        /// 帧解码
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public RoadGateFrame Decode(byte[] array,int len)
        {
            RoadGateFrame frame = new RoadGateFrame();

            byte[] buffer = DeEscape(array,len);

            using (MemoryStream mem = new MemoryStream(buffer))
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
        /// 添加转义字符
        /// 数据帧开始、结束标志为 FFH。其他字段不能出现 FFH，如果数据确实为 FFH，需对其进行转义处理。
        /// 发送数据时，如果在其它字段中出现 FFH 字节时，将 FFH 分解为 FEH 和 01H 这两个字节来发送；
        /// 如果在其它字段出现 FEH 字节时，需将 FEH 分解为 FEH 和 00H 这两个字节来发送。
        /// 接收数据时，如果出现“FE 01”这样连续两个字节时将之合为一个字节 FFH；
        /// 如果出现“FE 00”这样连续两个字节时将之合为一个字节 FEH。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] EnEscape(byte[] array, int len)
        {
            List<byte> list = new List<byte>();
            list.Add(array[0]);
            list.Add(array[1]);

            for (int i = 2; i < len - 1; i++)
            {
                switch (array[i])
                {
                    case 0xff:
                        list.Add(0xfe);
                        list.Add(0x01);
                        break;
                    case 0xfe:
                        list.Add(0xfe);
                        list.Add(0x00);
                        break;
                    default:
                        list.Add(array[i]);
                        break;
                }

            }

            list.Add(array[len - 1]);

            return list.ToArray();
        }


        /// <summary>
        /// 去除转义字符
        /// 数据帧开始、结束标志为 FFH。其他字段不能出现 FFH，如果数据确实为 FFH，需对其进行转义处理。
        /// 发送数据时，如果在其它字段中出现 FFH 字节时，将 FFH 分解为 FEH 和 01H 这两个字节来发送；
        /// 如果在其它字段出现 FEH 字节时，需将 FEH 分解为 FEH 和 00H 这两个字节来发送。
        /// 接收数据时，如果出现“FE 01”这样连续两个字节时将之合为一个字节 FFH；
        /// 如果出现“FE 00”这样连续两个字节时将之合为一个字节 FEH。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] DeEscape(byte[] array, int len)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                if (array[i] == 0xfe)
                {
                    switch (array[i + 1])
                    {
                        case 0x00:
                            list.Add(0xfe);
                            i++;
                            break;
                        case 0x01:
                            list.Add(0xff);
                            i++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    list.Add(array[i]);
                }
            }

            return list.ToArray();
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
