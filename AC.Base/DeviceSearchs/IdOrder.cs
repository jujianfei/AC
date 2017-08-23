using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按设备编号进行排序。
    /// </summary>
    public class IdOrder : IDeviceOrder
    {
        #region IOrder 成员

        /// <summary>
        /// 设备ID
        /// </summary>
        public string OrderNameAttribute
        {
            get { return "设备ID"; }
        }

        /// <summary>
        /// 该搜索排序器用于排序的字段。
        /// </summary>
        /// <returns></returns>
        public Searchs.SearchOrderColumn[] GetOrderColumns()
        {
            return new Searchs.SearchOrderColumn[] { new Searchs.SearchOrderColumn(Tables.Device.TableName, Tables.Device.DeviceId) };
        }

        #endregion
    }
}
