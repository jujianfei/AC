using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 日期选择参数。提供年月日的单日期选择界面。
    /// </summary>
    public interface IDay : IParameter
    {
        /// <summary>
        /// 设置日期。
        /// </summary>
        /// <param name="date">日期。</param>
        void SetParameterDay(DateTime date);
    }
}
