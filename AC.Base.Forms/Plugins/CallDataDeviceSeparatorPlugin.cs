using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;

namespace AC.Base.Forms.Plugins
{
    /// <summary>
    /// 召测设备数据 分隔符
    /// </summary>
    [AC.Base.Forms.DevicePluginType(null, 20000)]
    public class CallDataDeviceSeparatorPlugin : IDevicePlugin
    {
        #region IDevicePlugin 成员

        /// <summary>
        /// 设置该插件应显示或处理的设备。
        /// </summary>
        /// <param name="devices"></param>
        public void SetDevices(Device[] devices)
        {
        }

        #endregion

        #region IPlugin 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(FormApplicationClass application)
        {
        }

        #endregion
    }
}
