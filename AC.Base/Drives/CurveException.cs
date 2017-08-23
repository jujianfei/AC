using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 曲线数据异常。
    /// </summary>
    public class CurveException : Exception
    {
        /// <summary>
        /// 曲线数据异常。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public CurveException(string message)
            : base(message)
        {
        }
    }
}
