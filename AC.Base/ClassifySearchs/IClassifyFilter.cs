using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 分类搜索筛选器接口。
    /// </summary>
    public interface IClassifyFilter : Searchs.IFilter
    {
        /// <summary>
        /// 检查传入的分类是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="classify">被检查的分类。</param>
        /// <returns></returns>
        bool ClassifyFilterCheck(Classify classify);
    }
}
