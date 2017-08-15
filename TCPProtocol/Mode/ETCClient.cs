using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPProtocol
{
    class ETCClient:CommonThread
    {
        Socket socket;

        public ETCClient(string ip)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint point = new IPEndPoint(IPAddress.Parse(ip), 8081);

                socket.Connect(point);

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
                while(isRun==true)
                {

                }
            }
            catch(Exception e)
            {

            }
        }
    }
}
