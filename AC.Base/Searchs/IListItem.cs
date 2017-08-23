using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 在搜索结果列表中显示的某一列的内容。
    /// 实现该接口的类可以添加 IUseAccount 接口，表明该筛选器需要使用当前操作员账号对象。
    /// </summary>
    public interface IListItem
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        void SetApplication(ApplicationClass application);

        /// <summary>
        /// 搜索结果列的名称，用于显示在搜索结果列的列头。
        /// </summary>
        /// <returns></returns>
        string[] GetListItemNames();
    }
}
