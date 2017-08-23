using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 获取 IDayRangeCompare 日时段比较选择的字符串配置参数。
    /// </summary>
    public class DayRangeCompareParameter
    {
        /// <summary>
        /// 获取 IDayRange 日时段选择的字符串配置参数。
        /// </summary>
        /// <param name="startDate1">起始日期1。</param>
        /// <param name="endDate1">结束日期1。</param>
        /// <param name="startDate2">起始日期2。</param>
        /// <param name="endDate2">结束日期2。</param>
        /// <returns></returns>
        public static string GetValue(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            int intStartDateNum1 = startDate1.Year * 10000 + startDate1.Month * 100 + startDate1.Day;
            int intEndDateNum1 = endDate1.Year * 10000 + endDate1.Month * 100 + endDate1.Day;
            int intStartDateNum2 = startDate2.Year * 10000 + startDate2.Month * 100 + startDate2.Day;
            int intEndDateNum2 = endDate2.Year * 10000 + endDate2.Month * 100 + endDate2.Day;
            return intStartDateNum1.ToString() + "-" + intEndDateNum1.ToString() + "-" + intStartDateNum2.ToString() + "-" + intEndDateNum2.ToString();
        }
    }
}
