using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 设备经纬度列
    /// </summary>
    public class LongitudeLatitudeListItem
    {
        #region IDeviceListItem 成员

        /// <summary>
        /// 获取显示在设备搜索结果列中的内容。
        /// </summary>
        /// <param name="device">设备。</param>
        /// <param name="listItemName">列表项的名称。如果 IListItem.GetListItemNames() 方法只返回一项，此参数无意义。</param>
        /// <returns></returns>
        public string GetListItemValue(Device device, string listItemName)
        {
            if (device.Longitude != 0 && device.Latitude != 0)
            {
                return device.Longitude.ToString() + "," + device.Latitude.ToString();
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region IListItem 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
        }

        /// <summary>
        /// 搜索结果列的名称，用于显示在搜索结果列的列头。
        /// </summary>
        /// <returns></returns>
        public string[] GetListItemNames()
        {
            return new string[] { "经纬度" };
        }

        #endregion
    }
}
