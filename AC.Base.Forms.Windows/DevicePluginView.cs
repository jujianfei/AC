using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 设备插件视图。
    /// </summary>
    public class DevicePluginView : PluginViewBase
    {
        /// <summary>
        /// 设备插件视图。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="pluginType">插件类型。</param>
        /// <param name="devices">该视图需要呈现的设备对象。</param>
        public DevicePluginView(WindowsFormApplicationClass application, PluginType pluginType, Device[] devices)
            : base(application, pluginType)
        {
            this.Devices = devices;
        }

        /// <summary>
        /// 该视图所使用的设备。
        /// </summary>
        public Device[] Devices { get; private set; }

        /// <summary>
        /// 设置插件参数。PluginViewBase将插件初始化后调用该方法，由继承的类对插件进行一些设置后再进行后续的加载步骤。
        /// </summary>
        /// <param name="plugin">需要被设置参数的插件。</param>
        protected override void SetPlugin(IPlugin plugin)
        {
            IDevicePlugin devicePlugin = plugin as IDevicePlugin;
            devicePlugin.SetDevices(this.Devices);
        }

        /// <summary>
        /// 获取显示在视图标签页上的文字。
        /// </summary>
        /// <returns></returns>
        public override string GetViewTitle()
        {
            string strDeviceName = "";
            for (int intIndex = 0; intIndex < this.Devices.Length; intIndex++)
            {
                if (intIndex < 2)
                {
                    strDeviceName += "," + this.Devices[intIndex].Name;
                }
            }
            if (this.Devices.Length > 2)
            {
                strDeviceName += "...";
            }
            return this.PluginType.Name + " - " + strDeviceName.Substring(1);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strDeviceName = "";
            foreach (Device device in this.Devices)
            {
                strDeviceName += "," + device.Name;
            }
            return this.PluginType.Name + " - " + strDeviceName.Substring(1);
        }
    }
}
