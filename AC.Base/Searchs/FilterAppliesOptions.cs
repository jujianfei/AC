using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 筛选器应用场合选项。如某些特定的筛选器适合在设置权限时使用，放在搜索列表中则不太适合，设置筛选器此枚举的相应属性值使筛选器在特定应用场合中是否可操作。
    /// </summary>
    public enum FilterAppliesOptions
    {
        /// <summary>
        /// 不可以应用在任何场合。
        /// </summary>
        None = 0,

        /// <summary>
        /// 可以应用在任何场合。
        /// </summary>
        All = 0xFF,

        /// <summary>
        /// 可用于客户端软件中。如终端搜索、测量点搜索等。
        /// </summary>
        Client = 1 << 1,

        /// <summary>
        /// 可用于系统管理程序中。如设置操作员可操作的终端、可操作的测量点等。
        /// </summary>
        Manage = 1 << 2,

        /// <summary>
        /// 可用于系统配置程序中。如执行自动任务的设置项。
        /// </summary>
        SuperManage = 1 << 3,
    }
}
