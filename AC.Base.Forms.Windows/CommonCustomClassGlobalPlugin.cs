using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Forms.Plugins;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 所属分类管理
    /// </summary>
    [GlobalPluginType("所属分类管理", "", typeof(AdminGolbalSortplugin), 9, typeof(AC.Base.Forms.Windows.Manages.DepartmentManageIcon), null)]
    public class CommonCustomClassGlobalPlugin : ToolStripMenuItem, IGlobalPlugin
    {
        private WindowsFormApplicationClass m_Application;

         /// <summary>
        /// 初始化
        /// </summary>
        public CommonCustomClassGlobalPlugin()
        {
            base.Click += new EventHandler(CommonCustomClassGlobalPlugin_Click);
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(Forms.FormApplicationClass application)
        {
            this.m_Application = (WindowsFormApplicationClass)application;
        }

        void CommonCustomClassGlobalPlugin_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.Width = 650;
            f.Height = 400;
            f.StartPosition = FormStartPosition.CenterParent;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            f.ShowIcon = false;
            f.ShowInTaskbar = false;
            f.Text = "所属分类管理";
            Manages.CommonCustomClassifyManage ccm = new Manages.CommonCustomClassifyManage();
            ccm.SetApplication(m_Application);
            f.Controls.Add(ccm);
            ccm.Dock = DockStyle.Fill;
            f.ShowDialog();
        }

    }
}
