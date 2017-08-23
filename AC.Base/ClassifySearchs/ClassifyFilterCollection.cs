using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 分类筛选器集合。
    /// </summary>
    public class ClassifyFilterCollection : Searchs.FilterCollection<IClassifyFilter>, IClassifyFilter
    {
        #region IClassifyFilter 成员

        /// <summary>
        /// 检查传入的分类是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="classify">被检查的分类。</param>
        /// <returns></returns>
        public bool ClassifyFilterCheck(Classify classify)
        {
            if (this.Count > 0)
            {
                return this.ClassifyFilterCheck(this, classify);
            }
            else
            {
                return true;
            }
        }

        private bool ClassifyFilterCheck(Searchs.FilterCollection<IClassifyFilter> filters, Classify classify)
        {
            if (filters.LogicOperator == Searchs.FilterLogicOperatorOptions.And)
            {
                foreach (IClassifyFilter filter in filters)
                {
                    if (filter.ClassifyFilterCheck(classify) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (IClassifyFilter filter in filters)
                {
                    if (filter.ClassifyFilterCheck(classify))
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
