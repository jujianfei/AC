using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 设备搜索结果列表中显示的某一列的显示信息。
    /// </summary>
    public class DeviceListItemInfo : Searchs.ListItemInfo<IDeviceListItem>
    {
        /// <summary>
        /// 搜索结果列表中显示的某一列的显示信息。
        /// </summary>
        /// <param name="listItem">列表项。</param>
        public DeviceListItemInfo(IDeviceListItem listItem)
            : base(listItem)
        {
        }

        /// <summary>
        /// 搜索结果列表中显示的某一列的显示信息。
        /// </summary>
        /// <param name="listItem">列表项。</param>
        /// <param name="name">列表项名称。</param>
        public DeviceListItemInfo(IDeviceListItem listItem, string name)
            : base(listItem, name)
        {
        }

        /// <summary>
        /// 获取显示在设备搜索结果列中的内容。
        /// </summary>
        /// <param name="device">设备。</param>
        /// <returns></returns>
        public string GetListItemValue(Device device)
        {
            return base.ListItem.GetListItemValue(device, base.Name);
        }
    }
}
