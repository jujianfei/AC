using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备档案项描述。
    /// </summary>
    public class DeviceArchiveItemType
    {
        /// <summary>
        /// 设备档案项描述。
        /// </summary>
        /// <param name="type">当前档案项的类型声明。</param>
        internal DeviceArchiveItemType(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// 获取当前档案项的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 插件名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((DeviceArchiveItemTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 该插件的代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        private int m_OrdinalNumber = -1;
        /// <summary>
        /// 相对于当前插件集合内的排序顺序。按照数字从大到小的顺序列出功能项，如果使用默认值则功能项将在所有指定顺序号的功能项后列出。
        /// </summary>
        public int OrdinalNumber
        {
            get
            {
                if (this.m_OrdinalNumber == -1)
                {
                    this.m_OrdinalNumber = ((DeviceArchiveItemTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0]).OrdinalNumber;
                }
                return this.m_OrdinalNumber;
            }
        }

        /// <summary>
        /// 该插件可以使用在哪些类型的设备上。如果该值为null则表示可用在所有设备上。
        /// </summary>
        public Type ForDeviceType
        {
            get
            {
                if (this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false).Length > 0)
                {
                    DeviceArchiveItemTypeAttribute attr = this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;
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
                if (this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false).Length > 0)
                {
                    DeviceArchiveItemTypeAttribute attr = this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;
                    return attr.ForCheckType;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
