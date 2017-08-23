using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 日时段比较选择参数。提供两个起始日期至结束日期的选择界面，用于在两个时间段内对数据进行比较。
    /// </summary>
    public interface IDayRangeCompare : IParameter
    {
        /// <summary>
        /// 设置两个进行比较的日期范围。
        /// </summary>
        /// <param name="dateRange1">日期范围1</param>
        /// <param name="dateRange2">日期范围2</param>
        void SetParameterDayRangeCompare(DateRangeValue dateRange1, DateRangeValue dateRange2);
    }
}
