using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索筛选器所对应的界面控件。实现该接口的类必须添加 ControlAttribute 属性。
    /// </summary>
    public interface IFilterControl<F> : IControl
        where F : IFilter
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(ApplicationClass application);

        /// <summary>
        /// 设置搜索筛选器。
        /// </summary>
        /// <param name="filter">传入需要设置的搜索筛选器对象</param>
        void SetFilter(F filter);

        /// <summary>
        /// 获取搜索筛选器。操作员在界面上设置搜索条件后，由框架调用该方法获取设置后的搜索筛选器对象。
        /// </summary>
        /// <returns></returns>
        F GetFilter();

        /// <summary>
        /// 重置筛选器。清空设置值，使筛选器界面恢复默认状态。
        /// </summary>
        void ResetFilter();
    }
}
