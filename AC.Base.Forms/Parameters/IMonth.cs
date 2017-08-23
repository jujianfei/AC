using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Parameters
{
    /// <summary>
    /// 月份选择参数。提供年月的单日期选择界面。
    /// </summary>
    public interface IMonth : IParameter
    {
        /// <summary>
        /// 设置日期。
        /// </summary>
        /// <param name="date">日期。该日期中的年月数据有效。</param>
        void SetParameterMonth(DateTime date);
    }
}
