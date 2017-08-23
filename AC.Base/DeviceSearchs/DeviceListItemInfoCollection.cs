using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 设备搜索结果列表中显示信息的集合。
    /// </summary>
    public class DeviceListItemInfoCollection : Searchs.ListItemInfoCollection<DeviceListItemInfo, IDeviceListItem>
    {
        /// <summary>
        /// 向集合中添加搜索结果列表中显示的某一列的显示信息。
        /// </summary>
        /// <param name="listItem"></param>
        public void Add(IDeviceListItem listItem)
        {
            base.Add(new DeviceListItemInfo(listItem));
        }

        /// <summary>
        /// 向集合中添加搜索结果列表中显示的某一列的显示信息。
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="name"></param>
        public void Add(IDeviceListItem listItem, string name)
        {
            base.Add(new DeviceListItemInfo(listItem, name));
        }
    }
}
