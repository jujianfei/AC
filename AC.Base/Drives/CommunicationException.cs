using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 通讯异常。
    /// </summary>
    public class CommunicationException : Exception
    {
        /// <summary>
        /// 通讯异常。
        /// </summary>
        public CommunicationException()
        {

        }

        /// <summary>
        /// 通讯异常。
        /// </summary>
        /// <param name="message">异常消息。</param>
        public CommunicationException(string message)
            : base(message)
        {
        }
    }
}
