using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 搜索结果列表中显示的某一列的显示信息。
    /// </summary>
    public class ListItemInfo<L>
        where L : IListItem
    {
        /// <summary>
        /// 搜索结果列表中显示的某一列的显示信息。
        /// </summary>
        /// <param name="listItem">列表项。</param>
        public ListItemInfo(L listItem)
        {
            this.ListItem = listItem;
        }

        /// <summary>
        /// 搜索结果列表中显示的某一列的显示信息。
        /// </summary>
        /// <param name="listItem">列表项。</param>
        /// <param name="name">列表项名称。</param>
        public ListItemInfo(L listItem, string name)
        {
            this.ListItem = listItem;
            this.Name = name;
        }

        /// <summary>
        /// 列表项。
        /// </summary>
        public L ListItem { get; private set; }

        private string m_Name;
        /// <summary>
        /// 列表项名称。
        /// </summary>
        public string Name
        {
            get
            {
                if (this.m_Name == null || this.m_Name.Length == 0)
                {
                    this.m_Name = this.ListItem.GetListItemNames()[0];
                }
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

    }
}
