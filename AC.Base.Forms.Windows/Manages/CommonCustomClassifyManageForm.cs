using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Manages
{
    partial class CommonCustomClassifyManageForm : Form
    {
        public CommonCustomClassifyManageForm(CommonCustomClassify classify, bool isUpdate)
        {
            InitializeComponent();

            this.Classify = classify;

            if (isUpdate)
            {
                this.gbCommonClassify.Text = "修改公共分类";
            }
            else
            {
                this.gbCommonClassify.Text = "新建公共分类";
            }
        }

        public CommonCustomClassify Classify { get; private set; }

        private void CommonCustomClassifyManageForm_Load(object sender, EventArgs e)
        {
            this.txtName.Text = this.Classify.Name;
            this.chkEnabledDevice.Checked = this.Classify.EnabledDevice;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (this.txtName.Text.Length == 0)
            {
                MessageBox.Show("分类名称必须输入。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (MessageBox.Show("确实要" + this.gbCommonClassify.Text + "吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                this.Classify.Name = this.txtName.Text;
                this.Classify.EnabledDevice = this.chkEnabledDevice.Checked;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}
