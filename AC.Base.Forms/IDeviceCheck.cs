using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备插件检查接口。
    /// 实现IDevicePlugin接口并添加DevicePluginTypeAttribute特性的插件可以指定实现该接口的ForCheckType属性，当在设备上显示菜单前会初始化ForCheckType指向的类并调用Check方法。
    /// </summary>
    public interface IDeviceCheck
    {
        /// <summary>
        /// 检查设备插件能否用于指定的设备。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        bool Check(Device device);
    }
}
