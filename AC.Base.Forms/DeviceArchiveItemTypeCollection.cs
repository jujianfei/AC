using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 设备档案项描述集合。
    /// </summary>
    public class DeviceArchiveItemTypeCollection : ReadOnlyCollection<DeviceArchiveItemType>
    {
        internal DeviceArchiveItemTypeCollection()
        {
        }

        internal new void Add(DeviceArchiveItemType item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个档案项";
        }

    }
}
