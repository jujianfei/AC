using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 获取 IDayCompare 日比较选择的字符串配置参数。
    /// </summary>
    public class DayCompareParameter
    {
        /// <summary>
        /// 获取 IDayCompare 日比较选择的字符串配置参数。
        /// </summary>
        /// <param name="date1">日期1</param>
        /// <param name="date2">日期2</param>
        /// <returns></returns>
        public static string GetValue(DateTime date1, DateTime date2)
        {
            int intDateNum1 = date1.Year * 10000 + date1.Month * 100 + date1.Day;
            int intDateNum2 = date2.Year * 10000 + date2.Month * 100 + date2.Day;
            return intDateNum1.ToString() + "-" + intDateNum2.ToString();
        }
    }
}
