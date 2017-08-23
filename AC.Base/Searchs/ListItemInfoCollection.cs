using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索结果列表中显示信息的集合。
    /// </summary>
    /// <typeparam name="LI"></typeparam>
    /// <typeparam name="L"></typeparam>
    public class ListItemInfoCollection<LI, L> : System.Collections.Generic.List<LI>
        where LI : ListItemInfo<L>
        where L : IListItem
    {
    }
}
