using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 日时段选择参数。提供起始日期至结束日期的选择界面，用于查询统计连续的一段时间的数据。
    /// </summary>
    public interface IDayRange : IParameter
    {
        /// <summary>
        /// 设置日期范围。
        /// </summary>
        /// <param name="dateRange">日期范围。</param>
        void SetParameterDayRange(DateRangeValue dateRange);
    }
}
