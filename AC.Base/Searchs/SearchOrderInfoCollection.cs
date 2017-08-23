using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索器所使用的排序信息集合。
    /// </summary>
    /// <typeparam name="O"></typeparam>
    public class SearchOrderInfoCollection<O> : System.Collections.Generic.List<SearchOrderInfo<O>>
        where O : IOrder
    {
        /// <summary>
        /// 搜索器所使用的排序信息集合。
        /// </summary>
        public SearchOrderInfoCollection()
        {
        }

        /// <summary>
        /// 使用排序器初始化搜索器所使用的排序信息集合。
        /// </summary>
        /// <param name="orders">排序器。</param>
        public SearchOrderInfoCollection(params O[] orders)
        {
            foreach (O order in orders)
            {
                this.Add(order);
            }
        }

        /// <summary>
        /// 使用排序器及排序顺序初始化搜索器所使用的排序信息集合。
        /// </summary>
        /// <param name="isDesc">是否按倒序顺序排序。</param>
        /// <param name="orders">排序器。</param>
        public SearchOrderInfoCollection(bool isDesc, params O[] orders)
        {
            foreach (O order in orders)
            {
                this.Add(isDesc, order);
            }
        }

        /// <summary>
        /// 向集合内添加一个排序器。
        /// </summary>
        /// <param name="order">排序器。</param>
        public void Add(O order)
        {
            SearchOrderInfo<O> soi = new SearchOrderInfo<O>(order);
            this.Add(soi);
        }

        /// <summary>
        /// 向集合内添加一个排序器，并指明是否按倒序顺序排序
        /// </summary>
        /// <param name="isDesc">是否按倒序顺序排序。</param>
        /// <param name="order">排序器。</param>
        public void Add(bool isDesc, O order)
        {
            SearchOrderInfo<O> soi = new SearchOrderInfo<O>(order, isDesc);
            this.Add(soi);
        }
    }
}
