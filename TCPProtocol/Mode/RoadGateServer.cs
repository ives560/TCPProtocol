using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPProtocol
{
    class RoadGateServer:CommonThread
    {

        Socket server;


        public RoadGateServer(string ip)
        {
            server = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), 8080);

            try
            {
                server.Bind(point);
                server.Listen(10);

                Start();
            }
            catch(Exception e)
            {

            }

        }


        protected override void WorkerRun()
        {
            try
            {
                while (isRun == true)
                {
                    Socket client = server.Accept();
                    RoadGateClient roadgate = new RoadGateClient(client);

                }
            }
            catch(Exception e)
            {
                System.Console.Write(e.Message);
            }

        }
    }
}
