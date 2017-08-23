using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 设备列表菜单所使用的控件视图中的控件。
    /// </summary>
    public class DeviceListControlView : System.Windows.Forms.Control, IControlView, IUseAccount
    {
        private IAccount m_Account;

        #region IControlView 成员

        /// <summary>
        /// 返回当前控件视图的配置参数，以便下次打开该视图时可以通过 SetViewConfig 复原当前视图。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>如果当前控件视图无任何配置参数，则可以返回 null。</returns>
        public System.Xml.XmlNode GetViewConfig(System.Xml.XmlDocument xmlDoc)
        {
            return null;
        }

        /// <summary>
        /// 设置控件视图的配置参数。
        /// </summary>
        /// <param name="config"></param>
        public void SetViewConfig(System.Xml.XmlNode config)
        {
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(WindowsFormApplicationClass application)
        {
            AC.Base.DeviceSearchs.DeviceSearch search = new AC.Base.DeviceSearchs.DeviceSearch(application);
            search.Filters.Add(new AC.Base.DeviceSearchs.ParentIdFilter(0));
            //search.Filters.Add(new AC.Base.DeviceSearchs.StatusFilter(1));
            DeviceList deviceList = new DeviceList();
            deviceList.SetApplication(application);
            deviceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(deviceList);
            deviceList.SetDeviceSearch(search);
            deviceList.DeviceSearch(1);
        }

        #endregion

        #region IUseAccount 成员

        /// <summary>
        /// 设置当前操作员账号。
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(IAccount account)
        {
            this.m_Account = account;
        }

        #endregion
    }
}
