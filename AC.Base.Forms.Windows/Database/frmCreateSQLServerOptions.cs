using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Database
{
    partial class frmCreateSQLServerOptions : Form
    {
        public frmCreateSQLServerOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取或设置数据库名
        /// </summary>
        public string DatabaseName
        {
            get { return this.txtName.Text; }
            set { this.txtName.Text = value; }
        }

        /// <summary>
        /// 是否使用默认设置
        /// </summary>
        public bool IsDatabaseDefault
        {
            get { return this.chkDefault.Checked; }
            set { this.chkDefault.Checked = value; }
        }

        /// <summary>
        /// 数据文件存放路径
        /// </summary>
        public string DatabasePath
        {
            get { return this.txtPath.Text; }
            set { this.txtPath.Text = value; }
        }

        /// <summary>
        /// 数据文件初始大小
        /// </summary>
        public int DatabaseSize
        {
            get { return (int)this.numSize.Value; }
            set { this.numSize.Value = value; }
        }

        private void chkDefault_CheckedChanged(object sender, EventArgs e)
        {
            this.labPath.Enabled = !this.chkDefault.Checked;
            this.panPath.Enabled = !this.chkDefault.Checked;
            this.labSize.Enabled = !this.chkDefault.Checked;
            this.panSize.Enabled = !this.chkDefault.Checked;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "设置 SQL Server 数据库的数据文件(mdf)和日志文件(ldf)的保存位置。";
            fbd.SelectedPath = this.txtPath.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                this.txtPath.Text = fbd.SelectedPath;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (this.DatabaseName.Length == 0)
            {
                MessageBox.Show("请输入欲建立的数据库名。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtName.Focus();
                return;
            }

            if (MessageBox.Show("确实要创建数据库“" + this.DatabaseName + "”吗？", "创建 SQL Server 数据库", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
