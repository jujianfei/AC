using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 设备搜索筛选器所对应的界面控件。实现该接口的类必须添加 ControlAttribute 属性。
    /// </summary>
    public interface IDeviceFilterControl : Searchs.IFilterControl<IDeviceFilter>
    {
    }
}
