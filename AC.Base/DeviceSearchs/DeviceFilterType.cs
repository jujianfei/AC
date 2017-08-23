using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 用于设备搜索器进行条件筛选的设备筛选器。
    /// </summary>
    public class DeviceFilterType : Searchs.FilterType<IDeviceFilter>
    {
        /// <summary>
        /// 用于设备搜索器进行条件筛选的设备筛选器。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="type">设备筛选器类型。</param>
        public DeviceFilterType(ApplicationClass application, Type type)
            : base(application, type)
        {
        }
    }
}
