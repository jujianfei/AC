using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备插件。
    /// </summary>
    public class DevicePluginType : PluginType
    {
        internal DevicePluginType(ApplicationClass application, Type type)
            : base(application, type)
        {
        }

        /// <summary>
        /// 获取继承自 PluginTypeAttribute 的特性。
        /// </summary>
        protected override PluginTypeAttribute PluginTypeAttr
        {
            get { return (PluginTypeAttribute)this.Type.GetCustomAttributes(typeof(PluginTypeAttribute), false)[0]; }
        }

        /// <summary>
        /// 该插件支持的最大同时处理的设备数量（用于在设备列表选定多个设备后判断该插件是否在菜单上显示）。该值默认为1，表示该功能插件只支持一个设备；该值大于1表示该插件可以同时显示或处理不超过该数量的设备；该值为0表示对同时显示或处理的设备数量无限制。
        /// </summary>
        public int DeviceMaximum
        {
            get
            {
                if (base.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false).Length > 0)
                {
                    DevicePluginTypeAttribute attr = base.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false)[0] as DevicePluginTypeAttribute;
                    if (attr.DeviceMaximum < 1)
                    {
                        return 0;
                    }
                    else
                    {
                        return attr.DeviceMaximum;
                    }
                }

                return 1;
            }
        }

        /// <summary>
        /// 该插件可以使用在哪些类型的设备上。如果该值为null则表示可用在所有设备上。
        /// </summary>
        public Type ForDeviceType
        {
            get
            {
                if (base.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false).Length > 0)
                {
                    DevicePluginTypeAttribute attr = base.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false)[0] as DevicePluginTypeAttribute;
                    return attr.ForDeviceType;
                }

                return null;
            }
        }

        /// <summary>
        /// 通过该属性指向的类检查该插件能否用于指定的设备，指向的类必须实现IDeviceCheck接口。
        /// </summary>
        public Type ForCheckType
        {
            get
            {
                if (base.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false).Length > 0)
                {
                    DevicePluginTypeAttribute attr = base.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false)[0] as DevicePluginTypeAttribute;
                    return attr.ForCheckType;
                }

                return null;
            }
        }
    }
}
