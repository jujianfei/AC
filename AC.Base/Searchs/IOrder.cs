using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索排序器接口。
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// 获取排序器的名称。
        /// </summary>
        string OrderNameAttribute { get; }

        /// <summary>
        /// 该搜索排序器用于排序的字段。
        /// </summary>
        /// <returns></returns>
        SearchOrderColumn[] GetOrderColumns();
    }
}
