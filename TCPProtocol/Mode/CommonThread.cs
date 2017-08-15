using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPProtocol
{
    internal abstract class CommonThread
    {
        public Thread thread;
        protected bool isRun = true;
        protected CommonThread()
        {

        }
        public void Start()
        {
            isRun = true;

            thread = new Thread(WorkerRun);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Abort()
        {
            isRun = false;
            if(thread!=null&&thread.IsAlive)
            {
                thread.Join();
            }
        }

        protected abstract void WorkerRun();


    }
}
