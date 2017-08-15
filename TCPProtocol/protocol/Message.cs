using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtocol
{
     public class Message
     {
         private byte _class;//协议类别
         private byte _flag;//协议代码
         private int _size;//数据长度
         private byte[] _content;//实际数据

         public byte Class
         {
             get { return _class; }
             set { _class = value; }
         }

         public byte Flag
         {
             get { return _flag; }
             set { _flag = value; }
         }

         public int Size
         {
             get { return _size; }
             set { _size = value; }
         }

         public byte[] Content
         {
            get { return _content; }
            set { _content = value; }
        }


        public Message()
        {

        }

        public Message(byte @class, byte flag, byte[] content)
        {
            _class = @class;
            _flag = flag;
            _size = content.Length;
            _content = content;
        }

         //序列化
        public byte[] ToBytes()
        {
            byte[] _byte;
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                writer.Write(_class);
                writer.Write(_flag);
                writer.Write(_size);
                if (_size > 0)
                {
                    writer.Write(_content);
                }
                _byte = mem.ToArray();
                writer.Close();
            }
            return _byte;
        }

         //反序列化
        public static Message FromBytes(byte[] Buffer)
        {
            Message message = new Message();
            using (MemoryStream mem = new MemoryStream(Buffer))
            {
                BinaryReader reader = new BinaryReader(mem);
                message._class = reader.ReadByte();
                message._flag = reader.ReadByte();
                message._size = reader.ReadInt32();
                if (message._size > 0)
                {
                    message._content = reader.ReadBytes(message._size);
                }
                reader.Close();
            }
            return message;
        }

    }
}
