using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPProtocol
{
    public partial class Form1 : Form
    {
        RoadGateServer RoadGate;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            RoadGate = new RoadGateServer("10.9.4.102");
            //Message msge = new Message();
            //MessageStream stream = new MessageStream();
            //stream.Write(buffer, 0, 100);
            //stream.Read(out msge);
            

        }
    }
}
