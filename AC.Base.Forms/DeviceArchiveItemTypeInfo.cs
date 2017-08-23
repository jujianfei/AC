using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备档案项信息。
    /// </summary>
    internal class DeviceArchiveItemTypeInfo
    {
        internal DeviceArchiveItemTypeInfo(Type type)
        {
            this.Type = type;
            DeviceArchiveItemTypeAttribute attr = this.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;
            if (attr.OrdinalNumber > 0)
            {
                this.OrdinalNumber = attr.OrdinalNumber;
            }
            else
            {
                this.OrdinalNumber = Int32.MaxValue;
            }
            this.Inherits = new List<DeviceArchiveItemTypeInfo>();
        }

        /// <summary>
        /// 当前档案项类型。
        /// </summary>
        internal Type Type;

        /// <summary>
        /// 排序序号。
        /// </summary>
        internal int OrdinalNumber;

        /// <summary>
        /// 继承当前档案项的派生档案项集合。
        /// </summary>
        internal List<DeviceArchiveItemTypeInfo> Inherits;
    }
}
