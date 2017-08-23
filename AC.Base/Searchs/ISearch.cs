using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 实现数据库数据搜索功能的接口。
    /// </summary>
    /// <typeparam name="C">调用 Search 方法所返回的集合类型。该类型应该是一个可枚举的集合类。</typeparam>
    public interface ISearch<C>
    {
        /// <summary>
        /// 获取或设置每页显示的数据数量。当此属性设置为“0”时，则表示不进行分页，将所有符合条件的数据全部读取出。
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 开始搜索指定页数的数据。
        /// </summary>
        /// <param name="pageNum">搜索第几页的数据。当此参数为“0”时，则表示不进行分页，将所有符合条件的数据全部读取出。</param>
        /// <returns>搜索结果集合</returns>
        C Search(int pageNum);

        /// <summary>
        /// 获取当前是第几页的数据。
        /// </summary>
        int PageNum { get; }

        /// <summary>
        /// 获取此搜索结果数据的总页数。
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// 获取此搜索结果数据的总数据量。
        /// </summary>
        int RecordsetCount { get; }

        /// <summary>
        /// 当前页记录集数据在整个搜索结果中的开始索引，该值从0开始。
        /// </summary>
        int RecordsetStartIndex { get; }

        /// <summary>
        /// 当前页记录集数据在整个搜索结果中的结束索引，该值从0开始。
        /// </summary>
        int RecordsetEndIndex { get; }
    }
}
