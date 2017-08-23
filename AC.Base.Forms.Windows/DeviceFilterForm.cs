using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 提供筛选设备的筛选条件设置窗体，具有基本、高级两种模式。
    /// </summary>
    public class DeviceFilterForm : Form
    {
        private DeviceFilter ctlDeviceFilter;

        /// <summary>
        /// 提供筛选设备的筛选条件设置窗体，具有基本、高级两种模式。
        /// </summary>
        public DeviceFilterForm()
        {
            this.Padding = new Padding(8);
            this.Size = new Size(480, 540);
            this.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Text = "设置筛选条件";

            this.ctlDeviceFilter = new DeviceFilter();
            this.ctlDeviceFilter.Dock = DockStyle.Fill;
            this.Controls.Add(ctlDeviceFilter);

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
            this.CancelButton = btnCancel;

            panButton3.Dock = System.Windows.Forms.DockStyle.Left;
            panButton3.Location = new System.Drawing.Point(75, 0);
            panButton3.Size = new System.Drawing.Size(5, 23);

            btnAccept.Dock = System.Windows.Forms.DockStyle.Left;
            btnAccept.Location = new System.Drawing.Point(0, 0);
            btnAccept.Size = new System.Drawing.Size(75, 23);
            btnAccept.Text = "确定";
            btnAccept.Click += new EventHandler(btnAccept_Click);
            this.AcceptButton = btnAccept;

            this.Controls.Add(panButton);
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(ApplicationClass application)
        {
            this.ctlDeviceFilter.SetApplication(application);
        }

        /// <summary>
        /// 设置设备只读筛选器。只读筛选器通常用于强制性的添加默认搜索条件。
        /// </summary>
        /// <param name="filters"></param>
        public void SetReadonlyFilters(DeviceFilterCollection filters)
        {
            this.ctlDeviceFilter.SetReadonlyFilters(filters);
        }

        /// <summary>
        /// 获取或设置设备筛选器。
        /// </summary>
        public DeviceFilterCollection Filters
        {
            get
            {
                return this.ctlDeviceFilter.Filters;
            }
            set
            {
                this.ctlDeviceFilter.Filters = value;
            }
        }


        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定使用该筛选条件吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}
