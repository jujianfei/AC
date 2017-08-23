using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Forms.Windows.Manages;

namespace AC.Base.Forms.Windows.Manages
{
    class DepartmentManageForm : Form
    {
        private ClassifySetting m_ClassifySetting;

        public DepartmentManageForm(Department classify, bool isUpdate)
        {
            this.Classify = classify;

            GroupBox gb = new GroupBox();
            gb.Dock = DockStyle.Fill;
            gb.Text = "部门";
            this.Controls.Add(gb);

            this.m_ClassifySetting = new ClassifySetting();
            this.m_ClassifySetting.Dock = DockStyle.Fill;
            gb.Controls.Add(this.m_ClassifySetting);

            if (isUpdate)
            {
                this.Text = "修改部门";
            }
            else
            {
                this.Text = "新建部门";
            }

            this.m_ClassifySetting.Classify = this.Classify;

            Panel panButton = new System.Windows.Forms.Panel();
            Panel panButton2 = new System.Windows.Forms.Panel();
            Button btnCancel = new System.Windows.Forms.Button();
            Panel panButton3 = new System.Windows.Forms.Panel();
            Button btnAccept = new System.Windows.Forms.Button();

            panButton.Controls.Add(panButton2);
            panButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            panButton.Location = new System.Drawing.Point(8, 237);
            panButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            panButton.Size = new System.Drawing.Size(389, 28);

            panButton2.Controls.Add(btnCancel);
            panButton2.Controls.Add(panButton3);
            panButton2.Controls.Add(btnAccept);
            panButton2.Dock = System.Windows.Forms.DockStyle.Right;
            panButton2.Location = new System.Drawing.Point(226, 5);
            panButton2.Size = new System.Drawing.Size(163, 23);

            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            btnCancel.Location = new System.Drawing.Point(80, 0);
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.Text = "取消";

            panButton3.Dock = System.Windows.Forms.DockStyle.Left;
            panButton3.Location = new System.Drawing.Point(75, 0);
            panButton3.Size = new System.Drawing.Size(5, 23);

            btnAccept.Dock = System.Windows.Forms.DockStyle.Left;
            btnAccept.Location = new System.Drawing.Point(0, 0);
            btnAccept.Size = new System.Drawing.Size(75, 23);
            btnAccept.Text = "确定";
            btnAccept.Click += new EventHandler(btnAccept_Click);

            this.Controls.Add(panButton);

            this.AcceptButton = btnAccept;
            this.CancelButton = btnCancel;
            this.ClientSize = new System.Drawing.Size(482, 250);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
        }

        public Department Classify { get; private set; }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (this.m_ClassifySetting.GetClassifyName().Length == 0)
            {
                MessageBox.Show("部门名称必须输入。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (MessageBox.Show("确实要" + this.Text + "吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                this.Classify = this.m_ClassifySetting.Classify as Department;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DepartmentManageForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DepartmentManageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }
    }
}
