using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPProtocol
{
    class Entrance:CommonThread
    {
        ETCClient EtcClient;
        RoadGateServer RoadGate;
        public void Encoder()
        {

            RoadGate = new RoadGateServer("10.9.4.102");

            EtcClient = new ETCClient("10.9.4.102");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void WorkerRun()
        {
            while(isRun==true)
            {
               Thread.Sleep(0);
            }
        }


    }
}
