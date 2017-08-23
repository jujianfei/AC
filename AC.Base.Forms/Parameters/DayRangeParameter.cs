using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 获取 IDayRange 日时段选择的字符串配置参数。
    /// </summary>
    public class DayRangeParameter
    {
        /// <summary>
        /// 获取 IDayRange 日时段选择的字符串配置参数。
        /// </summary>
        /// <param name="startDate">起始日期。</param>
        /// <param name="endDate">结束日期。</param>
        /// <returns></returns>
        public static string GetValue(DateTime startDate, DateTime endDate)
        {
            int intStartDateNum = startDate.Year * 10000 + startDate.Month * 100 + startDate.Day;
            int intEndDateNum = endDate.Year * 10000 + endDate.Month * 100 + endDate.Day;
            return intStartDateNum.ToString() + "-" + intEndDateNum.ToString();
        }
    }
}
