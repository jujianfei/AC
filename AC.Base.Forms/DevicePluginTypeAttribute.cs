using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备插件特性。
    /// 添加该特性的类应实现 IDevicePlugin 或 IDeviceHtmlPlugin 接口。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DevicePluginTypeAttribute : PluginTypeAttribute
    {
        /// <summary>
        /// 该插件是一个终结插件。通常终结插件用于指明继承的插件在某种设备上不可用。
        /// </summary>
        public DevicePluginTypeAttribute()
            :base()
        {
        }
        
        /// <summary>
        /// 以分隔符或分组形式显示的全局插件分类。
        /// </summary>
        /// <param name="parentType">用于将插件分类的上级插件类型，如当前是根节点插件则可传入 null。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        public DevicePluginTypeAttribute(Type parentType, int ordinalNumber)
            : base(parentType, ordinalNumber)
        {
        }

        /// <summary>
        /// 设备插件特性。
        /// </summary>
        /// <param name="name">插件名称</param>
        /// <param name="description">有关该插件详细的说明。</param>
        /// <param name="parentType">用于将插件分类的上级插件类型。</param>
        /// <param name="ordinalNumber">相对于当前插件集合内的排序顺序。</param>
        /// <param name="imageType">该插件所使用的图标类，指向的类必须实现 AC.Base.IIcon 接口。</param>
        /// <param name="deviceMaximum">该插件支持的最大同时处理的设备数量（用于在设备列表选定多个设备后判断该插件是否在菜单上显示）。该值为1时，表示该功能插件只支持一个设备；该值大于1表示该插件可以同时显示或处理不超过该数量的设备；该值为0表示对同时显示或处理的设备数量无限制。</param>
        /// <param name="forDeviceOrCheckType">用于说明该插件可以用在何种类型的设备上，类型声明可以是设备或驱动所继承的基类或者实现的接口，也可以指向一个实现 IDeviceCheck 接口的类。</param>
        public DevicePluginTypeAttribute(string name, string description, Type parentType, int ordinalNumber , Type imageType, int deviceMaximum, Type forDeviceOrCheckType)
            : base(name, description, parentType, ordinalNumber , imageType)
        {
            this.DeviceMaximum = deviceMaximum;

            if (forDeviceOrCheckType != null)
            {
                if (forDeviceOrCheckType.GetInterface(typeof(IDeviceCheck).FullName) != null)
                {
                    this.ForCheckType = forDeviceOrCheckType;
                }
                else
                {
                    this.ForDeviceType = forDeviceOrCheckType;
                }
            }
        }

        /// <summary>
        /// 该插件支持的最大设备数量（用于在设备列表选定多个设备后判断该插件是否在菜单上显示）。该值为1时，表示该功能插件只支持一个设备；该值大于1表示该插件可以同时显示或处理不超过该数量的设备；该值为0表示对同时显示或处理的设备数量无限制。
        /// 该值默认为0，允许不限数量的设备。
        /// </summary>
        public int DeviceMaximum { get; private set; }

        /// <summary>
        /// 该插件可以使用在哪种类型的设备上。该类型是一个设备类型、设备已实现的接口、设备驱动类型或设备驱动已实现的接口。如果该值为 null 则表示可用在所有设备上。
        /// </summary>
        public Type ForDeviceType { get; private set; }

        /// <summary>
        /// 通过该属性指向的类检查该插件能否用于指定的设备，指向的类必须实现 IDeviceCheck 接口。
        /// </summary>
        public Type ForCheckType { get; private set; }
    }
}
