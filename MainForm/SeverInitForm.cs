using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MainForm
{
    public partial class SeverInitForm : Form
    {
        public SeverInitForm()
        {
            InitializeComponent();
        }

        private Channel sever;
        public void setSever(Channel sever)
        {
            this.sever = sever;
        }

        public void start()
        {
            if (this.sever != null)
            {

            }
        }

        private delegate void PBaddShowEventHandler(int index, int maxindex);
        private void PBaddShow_Invoke(int index, int maxindex)
        {
            if (index == 0)
            {
                progressBar1.Maximum = maxindex;
            }
            progressBar1.Value = index;
        }
        public void PBaddShow(int index, int maxindex)
        {
            progressBar1.BeginInvoke(new PBaddShowEventHandler(this.PBaddShow_Invoke), new object[] { index, maxindex });
        }

        public delegate void ThreadRunEventHandler();
        public event ThreadRunEventHandler EventThreadRun;
        private void OnThreadFun()
        {
            if (this.EventThreadRun != null)
            {
                this.EventThreadRun();
            }
        }

        object threadlock = new object();
        private void SeverInitForm_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(timer1_Tick);
            thread.Start();
        }
        private void timer1_Tick()
        {
            Thread.Sleep(1000);
            OnThreadFun();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
