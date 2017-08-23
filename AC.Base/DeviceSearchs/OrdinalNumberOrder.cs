using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按排序编号进行排序。
    /// </summary>
    public class OrdinalNumberOrder : IDeviceOrder
    {
        #region IOrder 成员

        /// <summary>
        /// 排序编号。
        /// </summary>
        public string OrderNameAttribute
        {
            get { return "排序编号"; }
        }

        /// <summary>
        /// 该搜索排序器用于排序的字段。
        /// </summary>
        /// <returns></returns>
        public AC.Base.Searchs.SearchOrderColumn[] GetOrderColumns()
        {
            return new Searchs.SearchOrderColumn[] { new Searchs.SearchOrderColumn(Tables.Device.TableName, Tables.Device.OrdinalNumber) };
        }

        #endregion
    }
}
