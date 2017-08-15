using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPProtocol
{
    class RoadGateClient : CommonThread
    {
        Socket client;

        public delegate void ClientCommandHandler(RoadGateFrame frame);
        /// <summary>
        /// 客户端数据完成事件
        /// </summary>
        public event ClientCommandHandler ClientCommand;


        public RoadGateClient(Socket s)
        {
            ClientCommand += RoadGateClient_ClientCommand;
            client = s;
            Start();
        }

        /// <summary>
        /// 线程工作函数
        /// </summary>
        protected override void WorkerRun()
        {
            try
            {
                RoadGateFrameCodec frameCodec = new RoadGateFrameCodec();
                bool timeout = false;
                //using(var timer = new Timer(_=>timeout=true,null,20000,Timeout.Infinite))
                //{
                    while (isRun == true && timeout == false)
                    {
                        RoadGateFrame frame = frameCodec.ReceiveFrame(client);
                        if(frame!=null)
                        {
                            System.Console.Write(frameCodec.FrameBuffer);
                            OnClientCommand(frame);
                        }

                        Thread.Sleep(0);
                    }

                    client.Close();
                //}

            }
            catch (Exception e)
            {
                client.Close();
                System.Console.Write(e.Message);
            } 
            
        }

        /// <summary>
        /// 触发客户端数据完成事件
        /// </summary>
        public void OnClientCommand(RoadGateFrame frame)
        {
            if (ClientCommand != null)
                ClientCommand(frame);
        }

        /// <summary>
        /// 接收数据处理事件
        /// </summary>
        /// <param name="frame"></param>
        void RoadGateClient_ClientCommand(RoadGateFrame frame)
        {
            switch (frame.cmdtype)
            {
                case 0:
                    break;
                default:
                    break;
            }
        }
    }
}
