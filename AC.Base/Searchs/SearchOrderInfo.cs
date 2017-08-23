using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 排序器信息。
    /// </summary>
    /// <typeparam name="O">搜索排序器类型。</typeparam>
    public class SearchOrderInfo<O> where O : IOrder
    {
        /// <summary>
        /// 排序器信息。
        /// </summary>
        /// <param name="order"></param>
        public SearchOrderInfo(O order)
        {
            this.Order = order;
            this.IsDesc = false;
        }

        /// <summary>
        /// 排序器信息。
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isDesc"></param>
        public SearchOrderInfo(O order, bool isDesc)
        {
            this.Order = order;
            this.IsDesc = isDesc;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 搜索排序器。
        /// </summary>
        public O Order { get; private set; }

        /// <summary>
        /// 是否按倒序顺序排序。
        /// </summary>
        public bool IsDesc { get; set; }

        /// <summary>
        /// 获取排序部分的SQL语句。
        /// </summary>
        /// <param name="enableTableName">是否需要显示数据表表名。</param>
        /// <param name="isReverse">是否颠倒排序顺序。</param>
        /// <returns></returns>
        public string GetSqlOrderBy(bool enableTableName, bool isReverse)
        {
            string strOrder = "";

            foreach (SearchOrderColumn column in this.Order.GetOrderColumns())
            {
                if (enableTableName)
                {
                    strOrder += "," + column.TableName + "." + column.ColumnName;
                }
                else
                {
                    strOrder += "," + column.ColumnName;
                }

                if ((this.IsDesc && (isReverse == false)) | (this.IsDesc == false && isReverse))
                {
                    strOrder += " DESC";
                }
            }

            if (strOrder.Length > 0)
            {
                strOrder = strOrder.Substring(1);
            }
            return strOrder;
        }
    }
}
