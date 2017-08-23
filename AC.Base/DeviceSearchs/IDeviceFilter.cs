using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 设备搜索筛选器接口。
    /// 实现该接口的类可以添加 IUseAccount 接口，表明该筛选器需要使用当前操作员账号对象。
    /// </summary>
    public interface IDeviceFilter : Searchs.IFilter
    {
        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        bool DeviceFilterCheck(Device device);
    }
}
