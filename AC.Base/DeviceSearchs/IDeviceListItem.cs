using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Searchs;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 在设备搜索结果列表中显示的某一列的内容。
    /// 实现该接口的类可以添加 IUseAccount 接口，表明该筛选器需要使用当前操作员账号对象。
    /// </summary>
    public interface IDeviceListItem : IListItem
    {
        /// <summary>
        /// 获取显示在设备搜索结果列中的内容。
        /// </summary>
        /// <param name="device">设备。</param>
        /// <param name="listItemName">列表项的名称。如果 IListItem.GetListItemNames() 方法只返回一项，此参数无意义。</param>
        /// <returns></returns>
        string GetListItemValue(Device device, string listItemName);
    }
}
