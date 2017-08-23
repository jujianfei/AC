using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MainForm
{
    public partial class frmWait : Form
    {
        public frmWait()
        {
            InitializeComponent();
        }
        public bool showis = false;
        private int _stop = 0;
        public int stop
        {
            get { return _stop; }
        }
        private void btnstop_Click(object sender, EventArgs e)
        {
            _stop = 1;
        }
    }
}