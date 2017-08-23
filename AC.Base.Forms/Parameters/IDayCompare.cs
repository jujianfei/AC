using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 日比较选择参数。提供“日期1”和“日期2”两个日期的选择界面，用于比较任意2天的数据。
    /// </summary>
    public interface IDayCompare : IParameter
    {
        /// <summary>
        /// 设置两个进行比较的日期。
        /// </summary>
        /// <param name="date1">日期1</param>
        /// <param name="date2">日期2</param>
        void SetParameterDayCompare(DateTime date1, DateTime date2);
    }
}
