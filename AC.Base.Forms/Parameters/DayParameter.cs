using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 获取 IDay 日期选择的字符串配置参数。
    /// </summary>
    public class DayParameter
    {
        /// <summary>
        /// 获取 IDay 日期选择的字符串配置参数。
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns></returns>
        public static string GetValue(int year, int month, int day)
        {
            int intDateNum = year * 10000 + month * 100 + day;
            return intDateNum.ToString();
        }

        /// <summary>
        /// 获取 IDay 日期选择的字符串配置参数。
        /// </summary>
        /// <param name="date">年月日的日期。</param>
        /// <returns></returns>
        public static string GetValue(DateTime date)
        {
            int intDateNum = date.Year * 10000 + date.Month * 100 + date.Day;
            return intDateNum.ToString();
        }
    }
}
