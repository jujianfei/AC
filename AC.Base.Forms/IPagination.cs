using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 界面处理完毕显示的内容后产生分页事件所用到的委托。
    /// </summary>
    /// <param name="recordsetCount">所显示数据在不分页的情况下总的记录数。</param>
    /// <param name="pageSize">每页显示多少条数据，“0”表示不分页。</param>
    public delegate void PaginationEventHandler(int recordsetCount, int pageSize);

    /// <summary>
    /// 功能项分页接口。WEB及SWT程序框架将为实现此接口的视图功能项、HTML功能项提供分页栏。包括上一页、下一页、跳转到指定页的按钮，及总页数、总记录数、当前页号的信息。
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// 界面加载时由应用程序调用该方法传入应该显示第几页的内容，首次调用时 pageNumber 为 1 。
        /// </summary>
        /// <param name="pageNumber"></param>
        void SetPageNumber(int pageNumber);

        /// <summary>
        /// 界面处理完毕显示的内容后产生分页事件，告知应用程序总记录数及页大小。
        /// </summary>
        event PaginationEventHandler Pagination;
    }
}
