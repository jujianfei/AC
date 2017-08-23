using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 设备筛选器集合。
    /// </summary>
    public class DeviceFilterCollection : Searchs.FilterCollection<IDeviceFilter>, IDeviceFilter
    {
        #region IDeviceFilter 成员

        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (this.Count > 0)
            {
                return this.DeviceFilterCheck(this, device);
            }
            else
            {
                return true;
            }
        }

        private bool DeviceFilterCheck(Searchs.FilterCollection<IDeviceFilter> filters, Device device)
        {
            if (filters.LogicOperator == Searchs.FilterLogicOperatorOptions.And)
            {
                foreach (IDeviceFilter filter in filters)
                {
                    if (filter.DeviceFilterCheck(device) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (IDeviceFilter filter in filters)
                {
                    if (filter.DeviceFilterCheck(device))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion
    }
}
