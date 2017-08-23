using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 用于分类搜索器进行条件筛选的分类筛选器。
    /// </summary>
    public class ClassifyFilterType : Searchs.FilterType<IClassifyFilter>
    {
        /// <summary>
        /// 用于分类搜索器进行条件筛选的分类筛选器。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="type">分类筛选器类型。</param>
        public ClassifyFilterType(ApplicationClass application, Type type)
            : base(application, type)
        {
        }
    }
}
