using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 按分类编号进行排序。
    /// </summary>
    public class IdOrder : IClassifyOrder
    {
        #region IOrder 成员

        /// <summary>
        /// 分类ID
        /// </summary>
        public string OrderNameAttribute
        {
            get { return "分类ID"; }
        }

        /// <summary>
        /// 该搜索排序器用于排序的字段。
        /// </summary>
        /// <returns></returns>
        public Searchs.SearchOrderColumn[] GetOrderColumns()
        {
            return new Searchs.SearchOrderColumn[] { new Searchs.SearchOrderColumn(Tables.Classify.TableName, Tables.Classify.ClassifyId) };
        }

        #endregion
    }
}
