using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Forms.Plugins;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 机构部门管理
    /// </summary>
    [GlobalPluginType("机构部门管理", "", typeof(AdminGolbalSortplugin), 9, typeof(AC.Base.Forms.Windows.Manages.DepartmentManageIcon), null)]
    public class DepartmentManageGlobalMenuplugin : ToolStripMenuItem, IGlobalPlugin
    {
        private WindowsFormApplicationClass m_Application;

        /// <summary>
        /// 初始化
        /// </summary>
        public DepartmentManageGlobalMenuplugin()
        {
            base.Click += new EventHandler(DepartmentManageGlobalMenuplugin_Click);
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(Forms.FormApplicationClass application)
        {
            this.m_Application = (WindowsFormApplicationClass)application;
        }

        void DepartmentManageGlobalMenuplugin_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.Width = 650;
            f.Height = 400;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.StartPosition = FormStartPosition.CenterParent;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            f.ShowIcon = false;
            f.ShowInTaskbar = false;
            f.Text = "机构部门管理";
            Manages.DepartmentManage dm = new Manages.DepartmentManage();
            dm.SetApplication(m_Application);
            f.Controls.Add(dm);
            dm.Dock = DockStyle.Fill;
            f.ShowDialog();
        }
    }
}
