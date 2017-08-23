using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms.Plugins;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 设备搜索视图
    /// </summary>
    [GlobalPluginType("设备搜索", "", typeof(ViewGlobalSortPlugin), 1, null, null)]
    public class DeviceSearchGlobalMenuPlugin : System.Windows.Forms.ToolStripMenuItem, Forms.IGlobalPlugin, IUseAccount
    {
        private WindowsFormApplicationClass m_Application;
        private IAccount m_Account;

        /// <summary>
        /// 设备列表视图
        /// </summary>
        public DeviceSearchGlobalMenuPlugin()
        {
            base.Click += new EventHandler(DeviceSearchMenuItem_Click);
        }

        #region IPlugin 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(FormApplicationClass application)
        {
            this.m_Application = (WindowsFormApplicationClass)application;
            this.Image = Properties.Resources.DeviceSearch16;
        }

        #endregion

        #region IUseAccount 成员

        /// <summary>
        /// 设置当前操作员。
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(IAccount account)
        {
            this.m_Account = account;
        }

        #endregion

        private void DeviceSearchMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ViewBase _View in this.m_Application.Views)
            {
                if (_View is ControlView)
                {
                    ControlView _ControlView = _View as ControlView;
                    if (_ControlView.GetControl() is DeviceSearchControlView)
                    {
                        _ControlView.Focus();
                        return;
                    }
                }
            }

            DeviceSearchControlView ctlDeviceSearch = new DeviceSearchControlView();
            ctlDeviceSearch.SetAccount(this.m_Account);
            ctlDeviceSearch.SetApplication(this.m_Application);
            ControlView view = new ControlView(this.m_Application, ctlDeviceSearch, "设备搜索", Properties.Resources.DeviceSearch16);
            this.m_Application.Views.Load(view, ViewDockOptions.Left);
        }
    }
}
