using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 获取 IMonth 日期选择的字符串配置参数。
    /// </summary>
    public class MonthParameter
    {
        /// <summary>
        /// 获取 IMonth 日期选择的字符串配置参数。
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public static string GetValue(int year, int month)
        {
            int intDateNum = year * 10000 + month * 100;
            return intDateNum.ToString();
        }

        /// <summary>
        /// 获取 IMonth 日期选择的字符串配置参数。
        /// </summary>
        /// <param name="month">年月日期。</param>
        /// <returns></returns>
        public static string GetValue(DateTime month)
        {
            int intDateNum = month.Year * 10000 + month.Month * 100;
            return intDateNum.ToString();
        }
    }
}
