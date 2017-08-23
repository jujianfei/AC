using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 档案项特性。
    /// 添加该特性的类应实现 IDeviceArchiveItem、IDeviceArchiveUpdateItem 或 IDeviceArchiveDeleteItem 接口。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DeviceArchiveItemTypeAttribute : System.Attribute
    {
        /// <summary>
        /// 档案项特性。
        /// 添加该特性的类应实现 IDeviceArchiveItem、IDeviceArchiveUpdateItem 或 IDeviceArchiveDeleteItem 接口。
        /// </summary>
        /// <param name="name">档案项名称。</param>
        /// <param name="ordinalNumber">相对于当前档案项集合内的排序顺序。按照数字从小到大的顺序列出档案项项，如果使用默认值则档案项将在所有指定顺序号的档案项后列出。</param>
        public DeviceArchiveItemTypeAttribute(string name, int ordinalNumber)
        {
            this.Name = name;
            this.OrdinalNumber = ordinalNumber;
        }

        /// <summary>
        /// 档案项特性。
        /// 添加该特性的类应实现 IDeviceArchiveItem、IDeviceArchiveUpdateItem 或 IDeviceArchiveDeleteItem 接口。
        /// </summary>
        /// <param name="name">档案项名称。</param>
        /// <param name="ordinalNumber">相对于当前档案项集合内的排序顺序。按照数字从小到大的顺序列出档案项项，如果使用默认值则档案项将在所有指定顺序号的档案项后列出。</param>
        /// <param name="forType">指定该档案项可以在哪种类型的设备上使用或指向插件检查的类（根据类型自动判断是指定设备类型还是设备检查类）。</param>
        public DeviceArchiveItemTypeAttribute(string name, int ordinalNumber, Type forType)
        {
            this.Name = name;
            this.OrdinalNumber = ordinalNumber;

            if (forType.GetInterface(typeof(IDeviceCheck).FullName) != null)
            {
                this.ForCheckType = forType;
            }
            else
            {
                this.ForDeviceType = forType;
            }
        }

        /// <summary>
        /// 档案项特性。
        /// 添加该特性的类应实现 IDeviceArchiveItem、IDeviceArchiveUpdateItem 或 IDeviceArchiveDeleteItem 接口。
        /// </summary>
        /// <param name="name">档案项名称。</param>
        /// <param name="ordinalNumber">相对于当前档案项集合内的排序顺序。按照数字从小到大的顺序列出档案项项，如果使用默认值则档案项将在所有指定顺序号的档案项后列出。</param>
        /// <param name="forDeviceType">该档案项可以使用在哪种类型的设备上。该类型应是一个设备类型、设备已实现的接口、设备驱动类型或设备驱动已实现的接口。如果该值为 null 则表示可用在所有设备上。</param>
        /// <param name="forCheckType">通过该属性指向的类检查该档案项能否用于指定的设备，指向的类必须实现 IDeviceCheck 接口。</param>
        public DeviceArchiveItemTypeAttribute(string name, int ordinalNumber, Type forDeviceType, Type forCheckType)
        {
            this.Name = name;
            this.OrdinalNumber = ordinalNumber;
            this.ForDeviceType = forDeviceType;
            this.ForCheckType = forCheckType;
        }

        /// <summary>
        /// 档案项名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 相对于当前档案项集合内的排序顺序。按照数字从小到大的顺序列出档案项项，如果使用默认值则档案项将在所有指定顺序号的档案项后列出。
        /// </summary>
        public int OrdinalNumber { get; private set; }

        /// <summary>
        /// 该档案项可以使用在哪种类型的设备上。该类型应是一个设备类型、设备已实现的接口、设备驱动类型或设备驱动已实现的接口。如果该值为 null 则表示可用在所有设备上。
        /// </summary>
        public Type ForDeviceType { get; private set; }

        /// <summary>
        /// 通过该属性指向的类检查该档案项能否用于指定的设备，指向的类必须实现 IDeviceCheck 接口。
        /// </summary>
        public Type ForCheckType { get; private set; }
    }
}
