using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base;

namespace AC.Base.Forms.Windows.Manages
{
    partial class RoadClassifyManageForm : Form
    {
        public RoadPosition Classify { get; private set; }

        public RoadClassifyManageForm(RoadPosition classify, bool isUpdate)
        {
            InitializeComponent();
            this.Classify = classify;
            if (isUpdate)
                this.txtName.Text = this.Classify.Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text.Length == 0)
            {
                MessageBox.Show("城市、路段灯杆名称必须输入。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (MessageBox.Show("确实要" + this.Text + "吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                this.Classify.Name = this.txtName.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (MessageBox.Show("确实要" + this.Text + "吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    this.Classify.Name = this.txtName.Text;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
            else if (e.KeyCode == Keys.Escape)
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
